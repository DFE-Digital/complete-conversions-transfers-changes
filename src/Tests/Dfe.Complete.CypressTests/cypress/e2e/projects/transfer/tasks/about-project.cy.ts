import { Logger } from "cypress/common/logger";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { dimensionsTrust, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import aboutTheProjectPageTransfer from "cypress/pages/projects/projectDetails/aboutTheProjectPageTransfer";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";

const project = ProjectBuilder.createTransferProjectRequest();
let projectId: string;
const schoolName = "Abbey College Manchester";
const localAuthority = "Manchester";
const region = "North West";
const incomingTrust = dimensionsTrust;
const outgoingTrust = macclesfieldTrust;

const projectFormAMat = ProjectBuilder.createTransferFormAMatProjectRequest();
let projectFormAMatId: string;
const formAMATSchoolName = "Priory Rise School";
const formAMATLocalAuthority = "Milton Keynes";
const formAMATRegion = "South East";

describe.skip("About the project page - transfer projects: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${projectFormAMat.urn.value}`);
        projectApi.createTransferProject(project).then((response) => (projectId = response.value));
        projectApi.createMatTransferProject(projectFormAMat).then((response) => (projectFormAMatId = response.value));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it.only("Should display the project details on the about project section for a transfer project", () => {
        Logger.log("Go to project");
        cy.visit(`projects/${projectId}/tasks`);

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

        Logger.log("About projects sections are displayed as expected");

        aboutTheProjectPageTransfer
            .hasProjectDetails(
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
                incomingTrust.companiesHouseNumber,
                incomingTrust.address,
                project.incomingTrustSharepointLink,
            )
            .hasOutgoingTrustDetails(
                outgoingTrust.name.toUpperCase(), // bug 208086,
                project.outgoingTrustUkprn.value,
                outgoingTrust.referenceNumber,
                outgoingTrust.companiesHouseNumber,
                outgoingTrust.address,
                project.outgoingTrustSharepointLink,
                project.outGoingTrustWillClose,
            );
    });

    it("Should display the project details on the about project section for a transfer form a MAT project", () => {
        Logger.log("Go to the about project section");
        cy.visit(`projects/${projectFormAMatId}/information`);

        Logger.log("Project summary details are still displayed at the top of the page");
        projectDetailsPage
            .containsHeading(formAMATSchoolName)
            .hasAcademyURNHeading(`${projectFormAMat.urn.value}`)
            .hasTransferTag()
            .hasFormAMATTag()
            .hasTransferDate(projectFormAMat.significantDate)
            .hasOutgoingTrust(outgoingTrust.name)
            .hasIncomingTrust(incomingTrust.name)
            .hasLAAndRegion(formAMATLocalAuthority, formAMATRegion)
            .hasSharePointLink(projectFormAMat.establishmentSharepointLink);

        Logger.log("About projects details sections are displayed as expected");
        aboutTheProjectPageTransfer
            .hasProjectDetails(
                projectFormAMat.significantDate,
                formAMATLocalAuthority,
                "Not applicable",
                formAMATRegion,
                "Not grouped",
            )
            .hasProjectAssignment(projectFormAMat.handingOverToRegionalCaseworkService!)
            .hasReasonsForTheTransfer(
                projectFormAMat.isDueTo2Ri!,
                projectFormAMat.isDueToInedaquateOfstedRating!,
                projectFormAMat.isDueToIssues!,
            )
            .hasAdvisoryBoardDetails(projectFormAMat.advisoryBoardDate, projectFormAMat.advisoryBoardConditions)
            .hasAcademyDetails(
                formAMATSchoolName,
                projectFormAMat.urn.value,
                "Academy converter",
                "3 to 11",
                "Primary",
                projectFormAMat.establishmentSharepointLink,
            )
            .hasIncomingTrustDetails(
                projectFormAMat.newTrustName.toUpperCase(), // bug 208086,
                incomingTrust.ukprn,
                projectFormAMat.newTrustReferenceNumber,
                incomingTrust.companiesHouseNumber,
                incomingTrust.address,
                projectFormAMat.incomingTrustSharepointLink,
            )
            .hasOutgoingTrustDetails(
                outgoingTrust.name.toUpperCase(), // bug 208086,
                projectFormAMat.outgoingTrustUkprn.value,
                outgoingTrust.referenceNumber,
                outgoingTrust.companiesHouseNumber,
                outgoingTrust.address,
                projectFormAMat.outgoingTrustSharepointLink,
                projectFormAMat.outGoingTrustWillClose,
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
            .pageHasMovedToSection("Incoming trust details")
            .jumpToSection("Outgoing trust details")
            .pageHasMovedToSection("Outgoing trust details");
    });
});
