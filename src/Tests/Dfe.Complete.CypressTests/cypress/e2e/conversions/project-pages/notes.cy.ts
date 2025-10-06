import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import notePage from "cypress/pages/projects/projectDetails/notePage";
import { todayFormatted } from "cypress/constants/stringTestConstants";
import { cypressUser, rdoLondonUser } from "cypress/constants/cypressConstants";
import validationComponent from "cypress/pages/validationComponent";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import noteApi from "cypress/api/noteApi";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: urnPool.conversion.whitchurch },
});
let projectId: string;

before(() => {
    projectRemover.removeProjectIfItExists(project.urn.value);
    projectApi.createMatConversionProject(project).then((response) => {
        projectId = response.value;
        noteApi.createNote(projectId, cypressUser.id, "My note to edit");
        noteApi.createNote(projectId, cypressUser.id, "My note to delete");
        noteApi.createNote(projectId, rdoLondonUser.id, "Other user note");
    });
});
beforeEach(() => {
    cy.login();
    cy.acceptCookies();
    cy.visit(`projects/${projectId}/notes`);
});

describe("Conversion Project Notes", () => {
    it("Should be able to add a note", () => {
        cy.visit(`projects/${projectId}/tasks`);
        Logger.log("Go to the notes section");
        projectDetailsPage.navigateTo("Notes").containsSubHeading("Notes");

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
            .withNote("My note to edit")
            .editNote()
            .hasLabel("Enter note")
            .contains("Do not include personal or financial information.")
            .noteTextboxHasValue("My note to edit")
            .enterNote("My note has been edited")
            .clickButton("Save note");

        Logger.log("Edited note is displayed");
        notePage
            .containsSuccessBannerWithMessage("Your note has been edited")
            .withNote("My note has been edited")
            .hasDate(todayFormatted)
            .hasUser(cypressUser.username);
    });

    it("Should be able to delete a note", () => {
        Logger.log("Delete the note");
        notePage
            .withNote("My note to delete")
            .editNote()
            .clickButton("Delete")
            .containsHeading("Are you sure you want to delete this note?")
            .contains(
                "This will remove the note from the project. If this note contains outdated information, it may be better to leave a new note with an update instead.",
            )
            .clickButton("Delete");

        Logger.log("Note is deleted successfully");
        notePage.containsSuccessBannerWithMessage("Your note has been deleted").doesntContain("My note to delete");
    });

    it("Should not be able to add a note with empty text", () => {
        Logger.log("Try to add a note with empty text");
        notePage.clickButton("Add note").clickButton("Add note");

        Logger.log("Error message is displayed");
        validationComponent.hasLinkedValidationError("The note field is required.");
    });

    it("Should not be able to edit another user's note", () => {
        Logger.log("Confirm edit button is not visible for another user's note");
        notePage.withNote("Other user note").hasDate(todayFormatted).hasUser(rdoLondonUser.username).noEditNoteLink();
    });

    it("Show no notes message when there are no notes", () => {
        Logger.log("Remove all notes for the project via API");
        noteApi.removeAllNotesForProject(projectId);

        Logger.log("Go to the notes section and check for no notes message");
        projectDetailsPage
            .navigateTo("Notes")
            .containsSubHeading("Notes")
            .contains("There are not any notes for this project yet.");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
