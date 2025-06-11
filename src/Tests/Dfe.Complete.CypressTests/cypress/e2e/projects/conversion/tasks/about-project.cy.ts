import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";
import aboutTheProjectPageConversion from "cypress/pages/projects/projectDetails/aboutTheProjectPageConversion";

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;
const schoolName = "Whitchurch Primary School";
const localAuthority = "Bath and North East Somerset";
const region = "South West";
describe("About a project - conversion project", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
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
});
