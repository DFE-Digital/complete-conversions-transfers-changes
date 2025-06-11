import { Logger } from "cypress/common/logger";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";
import { dimensionsTrust, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";

const project = ProjectBuilder.createTransferProjectRequest();
let projectId: string;
const schoolName = "Abbey College Manchester";
const localAuthority = "Manchester";
const region = "North West";
const incomingTrust = dimensionsTrust;
const outgoingTrust = macclesfieldTrust;

describe("About a transfer project", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createTransferProject(project).then((response) => (projectId = response.value));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks`);
    });

    it("Should display the project details on the about project section", () => {
        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Project summary details are still displayed at the top of the page");
        projectDetailsPage
            .containsHeading(schoolName)
            .hasAcademyURNHeading(`${project.urn.value}`)
            .hasTransferTag()
            .hasInAGroupTag()
            .hasTransferDate(project.significantDate)
            .hasOutgoingTrust(outgoingTrust.name)
            .hasIncomingTrust(incomingTrust.name)
            .hasLAAndRegion(localAuthority, region)
            .hasSharePointLink(project.establishmentSharepointLink);

        Logger.log("Project details sections are displayed as expected");

        aboutTheProjectPage
            .hasProjectDetails(
                "Transfer",
                project.significantDate,
                localAuthority,
                "Not applicable",
                region,
                project.groupReferenceNumber!,
            )
            .hasProjectAssignment(project.handingOverToRegionalCaseworkService!)
            .hasReasonsForTheTransfer(
                project.isDueTo2Ri!,
                project.isDueToInedaquateOfstedRating!,
                project.isDueToIssues!,
            )
            .hasAdvisoryBoardDetails(project.advisoryBoardDate, project.advisoryBoardConditions)
            .hasAcademyDetails(
                schoolName,
                project.urn.value,
                "Other independent school",
                "14 to 23",
                "Not applicable",
                project.establishmentSharepointLink,
            )
            .hasIncomingTrustDetails(
                incomingTrust.name.toUpperCase(), // bug 208086,
                project.incomingTrustUkprn.value,
                incomingTrust.referenceNumber,
                incomingTrust.number,
                incomingTrust.address,
                project.incomingTrustSharepointLink,
            )
            .hasOutgoingTrustDetails(
                outgoingTrust.name.toUpperCase(), // bug 208086,
                project.outgoingTrustUkprn.value,
                outgoingTrust.referenceNumber,
                outgoingTrust.number,
                outgoingTrust.address,
                project.outgoingTrustSharepointLink,
                project.outGoingTrustWillClose,
            );
    });
});
