import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";
import aboutTheProjectPageConversion from "cypress/pages/projects/projectDetails/aboutTheProjectPageConversion";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;
const schoolName = "Whitchurch Primary School";
const localAuthority = "Bath and North East Somerset";
const region = "South West";
const teammatesProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: 147801 },
    userAdId: rdoLondonUser.adId,
});
let teammatesProjectId: string;
describe.skip("About the project page - conversion projects: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${teammatesProject.urn.value}`);
        projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
        projectApi
            .createMatConversionProject(teammatesProject, rdoLondonUser.email)
            .then((response) => (teammatesProjectId = response.value));
    });
    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks`);
    });

    it("Should display the project details on the about project section for Conversion form a MAT project", () => {
        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Project summary details are still displayed at the top of the page");
        projectDetailsPage
            .containsHeading(schoolName)
            .hasSchoolURNHeading(`${project.urn.value}`)
            .hasFormAMATTag()
            .hasConversionTag()
            .hasConversionDate(project.significantDate)
            .hasIncomingTrust(macclesfieldTrust.name)
            .hasLAAndRegion(localAuthority, region)
            .hasSharePointLinks(project.establishmentSharepointLink, project.incomingTrustSharepointLink);

        Logger.log("Project details sections are displayed as expected");

        aboutTheProjectPageConversion
            .hasProjectDetails(project.significantDate, localAuthority, "Not applicable", region, "Not grouped")
            .hasProjectAssignment(project.handingOverToRegionalCaseworkService)
            .hasReasonsForTheConversion(project.hasAcademyOrderBeenIssued, project.isDueTo2Ri)
            .hasAdvisoryBoardDetails(project.advisoryBoardDate, project.advisoryBoardConditions)
            .hasSchoolDetails(
                schoolName,
                project.urn.value,
                "Academy converter",
                "4 to 11",
                "Primary",
                project.establishmentSharepointLink,
                "Bristol",
            )
            .hasNoAcademyDetailsAsURNNotProvided()
            .hasIncomingTrustDetails(
                macclesfieldTrust.name,
                macclesfieldTrust.ukprn,
                macclesfieldTrust.referenceNumber,
                macclesfieldTrust.companiesHouseNumber,
                macclesfieldTrust.address,
                project.incomingTrustSharepointLink,
            );
    });

    it("Should display page links that navigate to different sections of the about project page", () => {
        Logger.log("Go to the about project section");
        cy.visit(`projects/${projectId}/information`);

        Logger.log("Check that the page links correctly navigate to the different sections");
        aboutTheProjectPage
            .jumpToSection("Project details")
            .pageHasMovedToSection("Project details")
            .jumpToSection("Project assignment")
            .pageHasMovedToSection("Project assignment")
            .jumpToSection("Reasons for transfer")
            .pageHasMovedToSection("Reasons for transfer")
            .jumpToSection("Advisory board details")
            .pageHasMovedToSection("Advisory board details")
            .jumpToSection("Academy details")
            .pageHasMovedToSection("Academy details")
            .jumpToSection("Incoming trust details")
            .pageHasMovedToSection("Incoming trust details");
    });

    it("Should display 'Not assigned to project' banner when viewing a project that is not assigned to the user", () => {
        Logger.log("Go to unassigned project");
        cy.visit(`projects/${teammatesProjectId}/tasks`);

        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Check that the 'Not assigned to project' banner is displayed");
        projectDetailsPage.containsImportantBannerWithMessage(
            "Not assigned to project",
            "This project is not assigned to you and cannot be changed, you can add notes or contacts if required.",
        );
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
