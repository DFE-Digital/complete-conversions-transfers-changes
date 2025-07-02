import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import notePage from "cypress/pages/projects/projectDetails/notePage";
import { todayFormatted } from "cypress/constants/stringTestConstants";
import { cypressUser } from "cypress/constants/cypressConstants";
import validationComponent from "cypress/pages/validationComponent";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;

before(() => {
    projectRemover.removeProjectIfItExists(`${project.urn.value}`);
    projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
});
beforeEach(() => {
    cy.login();
    cy.acceptCookies();
    cy.visit(`projects/${projectId}/notes`);
});

// NOTE: these tests are currently chained, in that they depend on each other.
// this is not preferred practice, but there is currently no way to manipulate this data
describe("Conversion Project Notes", () => {
    it("Should be able to add a note", () => {
        cy.visit(`projects/${projectId}/tasks`);
        Logger.log("Go to the notes section");
        projectDetailsPage
            .navigateTo("Notes")
            .containsSubHeading("Notes")
            .contains("There are not any notes for this project yet.");

        Logger.log("Add a note");
        notePage
            .clickButton("Add note")
            .hasLabel("Enter note")
            .contains("Do not include personal or financial information.")
            .enterNote("This is a test note")
            .clickButton("Add note");

        Logger.log("Note is added successfully");
        notePage
            .containsSuccessBannerWithMessage("Your note has been added")
            .withNote("This is a test note")
            .hasDate(todayFormatted)
            .hasUser(cypressUser.username);
    });

    it("Should be able to edit a note", () => {
        Logger.log("Edit the note");
        notePage
            .withNote("This is a test note")
            .editNote()
            .hasLabel("Enter note")
            .contains("Do not include personal or financial information.")
            .noteTextboxHasValue("This is a test note")
            .enterNote("This is an edited test note")
            .clickButton("Save note");

        Logger.log("Edited note is displayed");
        notePage
            .containsSuccessBannerWithMessage("Your note has been edited")
            .withNote("This is an edited test note")
            .hasDate(todayFormatted)
            .hasUser(cypressUser.username);
    });

    it("Should be able to delete a note", () => {
        Logger.log("Delete the note");
        notePage
            .withNote("This is an edited test note")
            .editNote()
            .clickButton("Delete")
            .containsHeading("Are you sure you want to delete this note?")
            .contains(
                "This will remove the note from the project. If this note contains outdated information, it may be better to leave a new note with an update instead.",
            )
            .clickButton("Delete");

        Logger.log("Note is deleted successfully");
        notePage
            .containsSuccessBannerWithMessage("Your note has been deleted")
            .contains("There are not any notes for this project yet.");
    });

    it("Should not be able to add a note with empty text", () => {
        Logger.log("Try to add a note with empty text");
        notePage.clickButton("Add note").clickButton("Add note");

        Logger.log("Error message is displayed");
        validationComponent.hasLinkedValidationError("The note field is required.");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
