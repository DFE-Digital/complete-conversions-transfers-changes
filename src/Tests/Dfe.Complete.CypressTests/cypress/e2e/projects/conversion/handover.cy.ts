import projectRemover from "cypress/api/projectRemover";
import { PrepareProjectBuilder } from "cypress/api/prepareProjectBuilder";
import prepareProjectApi from "cypress/api/prepareProjectApi";
import { beforeEach } from "mocha";
import { Logger } from "cypress/common/logger";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { dimensionsTrust } from "cypress/constants/stringTestConstants";
import { significateDateToDisplayDate } from "cypress/support/formatDate";
import { cypressUser, regionalCaseworkerTeamLeaderUser } from "cypress/constants/cypressConstants";
import conversionHandoverForm from "cypress/pages/projects/handover/conversionHandoverForm";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import yourProjects from "cypress/pages/projects/yourProjects";
import validationComponent from "cypress/pages/validationComponent";

const project = PrepareProjectBuilder.createConversionProjectRequest({ urn: 151111 });
const schoolName = "Our Lady of Walsingham Primary School";

const formAMATProject = PrepareProjectBuilder.createConversionFormAMatProjectRequest({ urn: 151113 });
const formAMATSchoolName = "Hope Brook CofE Primary School";

const otherProject = PrepareProjectBuilder.createConversionProjectRequest({ urn: 151114 });
const otherSchoolName = "Moor Park Primary School";

describe("Handover process tests for conversion projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn}`);
        projectRemover.removeProjectIfItExists(`${formAMATProject.urn}`);
        projectRemover.removeProjectIfItExists(`${otherProject.urn}`);
        prepareProjectApi.createConversionProject(project);
        prepareProjectApi.createConversionFormAMatProject(formAMATProject);
        prepareProjectApi.createConversionProject(otherProject);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`/projects/all/handover`);
    });

    it("Should be able to handover a conversion project to RCS", () => {
        Logger.log("Add handover details for project");
        projectTable.goToNextPageUntilFieldIsVisible(schoolName).clickButtonInRow(schoolName, "Add handover details");

        Logger.log("Verify details for project");
        projectDetailsPage
            .inOrder()
            .summaryShows("School name")
            .hasValue(schoolName)
            .summaryShows("URN")
            .hasValue(`${project.urn}`)
            .summaryShows("Project type")
            .hasValue("Conversion")
            .summaryShows("Form a MAT?")
            .hasValue("No")
            .summaryShows("Incoming trust name")
            .hasValue(dimensionsTrust.name)
            .summaryShows("Incoming trust UKPRN")
            .hasValue(dimensionsTrust.ukprn)
            .summaryShows("Advisory board date")
            .hasValue(significateDateToDisplayDate(project.advisory_board_date))
            .summaryShows("Provisional transfer date") // todo bug
            .hasValue(significateDateToDisplayDate(project.provisional_conversion_date))
            .summaryShows("Type of academy order")
            .hasValue("AO (Academy order)")
            .summaryShows("Assigned to in Prepare")
            .hasValue(cypressUser.username)
            .clickButton("Confirm");

        Logger.log("Complete handover form");
        conversionHandoverForm
            .withWillRCSManageProject("Yes")
            .withHandoverComments("Handover comments")
            .withSchoolSharePointLink("https://educationgovuk-my.sharepoint.com/1")
            .withIncomingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/2")
            .withDueToInterventionFollowing2RI("No")
            .confirm();

        Logger.log("Verify project handed over");
        cy.contains("Project handed over to Regional Casework Services");
        cy.contains(`${schoolName} URN ${project.urn}`);
        // cy.contains("This project will appear in the Regional Casework Services' project list."); //todo bug typo
        // cy.contains("It will only be assigned to a caseworker when external contacts have been added."); // todo bug

        Logger.log("Login with RCS team leader user");
        cy.clearCookies();
        cy.login(regionalCaseworkerTeamLeaderUser);
        cy.visit("/");

        Logger.log("Verify project is in unassigned projects for RCS");
        yourTeamProjects.goToNextPageUntilFieldIsVisible(schoolName);
    });

    it("Should be able to handover a conversion form a MAT project to assign to the user", () => {
        Logger.log("Add handover details for project");
        projectTable
            .goToNextPageUntilFieldIsVisible(formAMATSchoolName)
            .clickButtonInRow(formAMATSchoolName, "Add handover details");

        Logger.log("Verify details for project");
        projectDetailsPage
            .inOrder()
            .summaryShows("School name")
            .hasValue(formAMATSchoolName)
            .summaryShows("URN")
            .hasValue(`${formAMATProject.urn}`)
            .summaryShows("Project type")
            .hasValue("Conversion")
            .summaryShows("Form a MAT?")
            .hasValue("Yes")
            .summaryShows("New trust")
            .hasValue(dimensionsTrust.name)
            .summaryShows("Trust reference number")
            .hasValue(dimensionsTrust.referenceNumber)
            .summaryShows("Advisory board date")
            .hasValue(significateDateToDisplayDate(formAMATProject.advisory_board_date))
            .summaryShows("Provisional transfer date") // todo bug
            .hasValue(significateDateToDisplayDate(formAMATProject.provisional_conversion_date))
            .summaryShows("Type of academy order")
            .hasValue("AO (Academy order)")
            .summaryShows("Assigned to in Prepare")
            .hasValue(cypressUser.username)
            .clickButton("Confirm");

        Logger.log("Complete handover form");
        conversionHandoverForm
            .withWillRCSManageProject("No")
            .withSchoolSharePointLink("https://educationgovuk-my.sharepoint.com/1")
            .withIncomingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/2")
            .withDueToInterventionFollowing2RI("Yes")
            .confirm();

        Logger.log("Verify project handed over");
        cy.contains("Project assigned");
        cy.contains(`${formAMATSchoolName} URN ${formAMATProject.urn}`);
        cy.contains("This project will appear in your project list.");
        cy.contains("You can assign it to a different delivery officer if you will not work on it any more.");

        Logger.log("Verify project assigned to the user in your projects");
        cy.visit("/projects/yours/in-progress");
        yourProjects.goToNextPageUntilFieldIsVisible(formAMATSchoolName);
    });

    it("Should show multiple field validation errors when trying to continue with invalid input", () => {
        Logger.log("Add handover details for project and confirm details");
        projectTable
            .goToNextPageUntilFieldIsVisible(otherSchoolName)
            .clickButtonInRow(otherSchoolName, "Add handover details");
        projectDetailsPage.clickButton("Confirm");

        Logger.log("Confirm without filling in the form");
        conversionHandoverForm.confirm();

        Logger.log("Verify validation errors are shown on form");
        validationComponent
            .hasLinkedValidationError(
                "State if this project will be handed over to the Regional casework services team. Choose yes or no",
            )
            .hasLinkedValidationError("Enter a school SharePoint link")
            .hasLinkedValidationError("Enter an incoming trust SharePoint link")
            .hasLinkedValidationError("Select yes or no");

        Logger.log("Verify RCS handover comments required when RCS will manage the project");
        conversionHandoverForm.withWillRCSManageProject("Yes").confirm();
        validationComponent.hasLinkedValidationError("Enter handover notes");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
