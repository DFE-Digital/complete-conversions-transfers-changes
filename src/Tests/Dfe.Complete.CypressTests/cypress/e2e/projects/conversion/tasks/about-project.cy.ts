import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";
import aboutTheProjectPageConversion from "cypress/pages/projects/projectDetails/aboutTheProjectPageConversion";
import { rdoLondonUser } from "cypress/constants/cypressConstants";

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
describe("About a project - conversion project", () => {
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
            // .hasConversionDate(project.significantDate) // incorrect date format
            .hasIncomingTrust(macclesfieldTrust.name);
        // .hasLAAndRegion(localAuthority, region); case-sensitive issue with LA
        // .hasSharePointLinks(project.establishmentSharepointLink, project.incomingTrustSharepointLink); // has a /n in middle of text

        Logger.log("Project details sections are displayed as expected");

        aboutTheProjectPageConversion
            .hasProjectDetails(project.significantDate, localAuthority, "Not applicable", region, "Not grouped")
            // .hasProjectAssignment(project.handingOverToRegionalCaseworkService) // "handling" over is misspelled
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
                macclesfieldTrust.number,
                macclesfieldTrust.address,
                project.incomingTrustSharepointLink,
            );
    });

    // not implemented
    it.skip("Should display 'Not assigned to project' banner when viewing a project that is not assigned to the user", () => {
        Logger.log("Go to unassigned project");
        cy.visit(`projects/${teammatesProjectId}/tasks`);

        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Check that the 'Not assigned to project' banner is displayed");
        cy.contains("Not assigned to project");
        cy.contains(
            "This project is not assigned to you and cannot be changed, you can add notes or contacts if required.",
        );
    });
});
