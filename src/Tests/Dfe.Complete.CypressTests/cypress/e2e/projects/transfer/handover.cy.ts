import projectRemover from "cypress/api/projectRemover";
import { PrepareProjectBuilder } from "cypress/api/prepareProjectBuilder";
import prepareProjectApi from "cypress/api/prepareProjectApi";
import { beforeEach } from "mocha";
import { Logger } from "cypress/common/logger";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { dimensionsTrust, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import { significateDateToDisplayDate } from "cypress/support/formatDate";
import { cypressUser, regionalCaseworkerTeamLeaderUser } from "cypress/constants/cypressConstants";
import transferHandoverForm from "cypress/pages/projects/handover/transferHandoverForm";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import yourProjects from "cypress/pages/projects/yourProjects";
import validationComponent from "cypress/pages/validationComponent";

const project = PrepareProjectBuilder.createTransferProjectRequest({ urn: 151115 });
const academyName = "Rusthall St Paul's CofE Primary School";

const formAMATProject = PrepareProjectBuilder.createTransferFormAMatProjectRequest({ urn: 151116 });
const formAMATAcademyName = "Two Mile Hill Primary School";

const otherProject = PrepareProjectBuilder.createTransferProjectRequest({ urn: 151118 });
const otherAcademyName = "Park View";

// skip as prepare endpoint not implemented in dotnet 214917
describe.skip("Handover process tests for transfer projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn}`);
        projectRemover.removeProjectIfItExists(`${formAMATProject.urn}`);
        projectRemover.removeProjectIfItExists(`${otherProject.urn}`);
        prepareProjectApi.createTransferProject(project);
        prepareProjectApi.createTransferFormAMatProject(formAMATProject);
        prepareProjectApi.createTransferProject(otherProject);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`/projects/all/handover`);
    });

    it("Should be able to handover a transfer project to RCS", () => {
        Logger.log("Add handover details for project");
        projectTable.goToNextPageUntilFieldIsVisible(academyName).clickButtonInRow(academyName, "Add handover details");

        Logger.log("Verify details for project");
        projectDetailsPage
            .inOrder()
            .summaryShows("Academy name")
            .hasValue(academyName)
            .summaryShows("URN")
            .hasValue(`${project.urn}`)
            .summaryShows("Project type")
            .hasValue("Transfer")
            .summaryShows("Form a MAT?")
            .hasValue("No")
            .summaryShows("Incoming trust name")
            .hasValue(dimensionsTrust.name)
            .summaryShows("Incoming trust UKPRN")
            .hasValue(dimensionsTrust.ukprn)
            .summaryShows("Outgoing trust name")
            .hasValue(macclesfieldTrust.name)
            .summaryShows("Outgoing trust UKPRN")
            .hasValue(macclesfieldTrust.ukprn)
            .summaryShows("Advisory board date")
            .hasValue(significateDateToDisplayDate(project.advisory_board_date))
            .summaryShows("Provisional transfer date")
            .hasValue(significateDateToDisplayDate(project.provisional_transfer_date))
            .summaryShows("Assigned to in Prepare")
            .hasValue(cypressUser.username)
            .clickButton("Confirm");

        Logger.log("Complete handover form");
        transferHandoverForm
            .withWillRCSManageProject("Yes")
            .withHandoverComments("Handover comments")
            .withSchoolSharePointLink("https://educationgovuk-my.sharepoint.com/1")
            .withIncomingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/2")
            .withOutgoingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/3")
            .confirm();

        Logger.log("Verify project handed over");
        cy.contains("Project handed over to Regional Casework Services");
        cy.contains(`${academyName} URN ${project.urn}`);
        // bug 234289
        // cy.contains("This project will appear in the Regional Casework Services' project list."); // typo

        // below is what was in ruby, but doesn't make any sense as the project appears in RCS unassigned projects
        // cy.contains("It will only be assigned to a caseworker when external contacts have been added.");

        // this would make more sense:
        // cy.contains("This project will appear in the Regional Casework Services' unassigned project list.")
        // cy.contains("Regional Casework Services team leaders can assign it to a caseworker.");

        Logger.log("Login with RCS team leader user");
        cy.clearCookies();
        cy.login(regionalCaseworkerTeamLeaderUser);
        cy.visit("/");

        Logger.log("Verify project is in unassigned projects for RCS");
        yourTeamProjects.goToNextPageUntilFieldIsVisible(academyName);
    });

    it("Should be able to handover a transfer form a MAT project to assign to the user", () => {
        Logger.log("Add handover details for project");
        projectTable
            .goToNextPageUntilFieldIsVisible(formAMATAcademyName)
            .clickButtonInRow(formAMATAcademyName, "Add handover details");

        Logger.log("Verify details for project");
        projectDetailsPage
            .inOrder()
            .summaryShows("Academy name")
            .hasValue(formAMATAcademyName)
            .summaryShows("URN")
            .hasValue(`${formAMATProject.urn}`)
            .summaryShows("Project type")
            .hasValue("Transfer")
            .summaryShows("Form a MAT?")
            .hasValue("Yes")
            .summaryShows("New trust")
            .hasValue(dimensionsTrust.name)
            .summaryShows("Trust reference number")
            .hasValue(dimensionsTrust.referenceNumber)
            .summaryShows("Outgoing trust name")
            .hasValue(macclesfieldTrust.name)
            .summaryShows("Outgoing trust UKPRN")
            .hasValue(macclesfieldTrust.ukprn)
            .summaryShows("Advisory board date")
            .hasValue(significateDateToDisplayDate(formAMATProject.advisory_board_date))
            .summaryShows("Provisional transfer date")
            .hasValue(significateDateToDisplayDate(formAMATProject.provisional_transfer_date))
            .summaryShows("Assigned to in Prepare")
            .hasValue(cypressUser.username)
            .clickButton("Confirm");

        Logger.log("Complete handover form");
        transferHandoverForm
            .withWillRCSManageProject("No")
            .withSchoolSharePointLink("https://educationgovuk-my.sharepoint.com/1")
            .withIncomingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/2")
            .withOutgoingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/3")
            .confirm();

        Logger.log("Verify project handed over");
        cy.contains("Project assigned");
        cy.contains(`${formAMATAcademyName} URN ${formAMATProject.urn}`);
        cy.contains("This project will appear in your project list.");
        cy.contains("You can assign it to a different delivery officer if you will not work on it any more.");

        Logger.log("Verify project assigned to the user in your projects");
        cy.visit("/projects/yours/in-progress");
        yourProjects.goToNextPageUntilFieldIsVisible(formAMATAcademyName);
    });

    it("Should show multiple field validation errors when trying to continue with invalid input", () => {
        Logger.log("Add handover details for project and confirm details");
        projectTable
            .goToNextPageUntilFieldIsVisible(otherAcademyName)
            .clickButtonInRow(otherAcademyName, "Add handover details");
        projectDetailsPage.clickButton("Confirm");

        Logger.log("Confirm without filling in the form");
        transferHandoverForm.confirm();

        Logger.log("Verify validation errors are shown on form");
        validationComponent
            .hasLinkedValidationError(
                "State if this project will be handed over to the Regional casework services team. Choose yes or no",
            )
            .hasLinkedValidationError("Enter a school SharePoint link")
            .hasLinkedValidationError("Enter an incoming trust SharePoint link")
            .hasLinkedValidationError("Enter an outgoing trust SharePoint link");

        Logger.log("Verify RCS handover comments required when RCS will manage the project");
        transferHandoverForm.withWillRCSManageProject("Yes").confirm();
        validationComponent.hasLinkedValidationError("Enter handover notes");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
