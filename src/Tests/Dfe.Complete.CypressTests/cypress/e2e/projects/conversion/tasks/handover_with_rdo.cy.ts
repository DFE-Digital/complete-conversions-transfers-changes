import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { cypressUser, rdoLondonUser } from "cypress/constants/cypressConstants";
import taskNote from "cypress/pages/projects/tasks/taskNote";
import { Logger } from "cypress/common/logger";
import { todayFormatted } from "cypress/constants/stringTestConstants";
import noteApi from "cypress/api/noteApi";
import validationComponent from "cypress/pages/validationComponent";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;

before(() => {
    projectRemover.removeProjectIfItExists(`${project.urn.value}`);
    projectApi.createMatConversionProject(project).then((response) => {
        projectId = response.value;
        noteApi.createNote(projectId, cypressUser.id, "My task note to edit", "Handover");
        noteApi.createNote(projectId, cypressUser.id, "My task note to delete", "Handover");
        noteApi.createNote(projectId, rdoLondonUser.id, "Other user task note", "Handover");
    });
});

beforeEach(() => {
    cy.login();
    cy.acceptCookies();
    cy.visit(`projects/${projectId}/tasks/handover`);
});

describe("Handover with Regional Delivery Officer task", () => {
    it("Should be able to add a new task note", () => {
        Logger.log("Add a new task note");

        taskNote
            .clickButton("Add a new task note")
            .contains("Enter note")
            .contains("Do not include personal or financial information.")
            .enterNote("This is a new test task note")
            .clickButton("Add note");

        Logger.log("Task note is added successfully");
        taskNote
            .containsSuccessBannerWithMessage("Your note has been added")
            .withNote("This is a new test task note")
            .hasDate(todayFormatted)
            .hasUser(cypressUser.username);
    });

    it("Should be able to edit an existing task note", () => {
        Logger.log("Edit the existing task note");

        taskNote
            .withNote("My task note to edit")
            .editNote()
            .contains("Enter note")
            .contains("Do not include personal or financial information.")
            .noteTextboxHasValue("My task note to edit")
            .enterNote("My note has been edited")
            .clickButton("Save note");

        Logger.log("Task note is updated successfully");
        taskNote
            .containsSuccessBannerWithMessage("Your note has been edited")
            .withNote("My note has been edited")
            .hasDate(todayFormatted)
            .hasUser(cypressUser.username);
    });

    it("Should be able to delete a task note", () => {
        Logger.log("Delete the task note");

        taskNote
            .withNote("My task note to delete")
            .editNote()
            .clickButton("Delete")
            .contains("Are you sure you want to delete this note?")
            .contains(
                "This will remove the note from the project. If this note contains outdated information, it may be better to leave a new note with an update instead.",
            )
            .clickButton("Delete");

        Logger.log("Task note is deleted successfully");
        taskNote.containsSuccessBannerWithMessage("Your note has been deleted").doesntContain("My note to delete");
    });

    it("Should not be able to add a task note with empty text", () => {
        Logger.log("Try to add a task note with empty text");
        taskNote.clickButton("Add a new task note").clickButton("Add note");

        Logger.log("Error message is displayed");
        validationComponent.hasLinkedValidationError("The note field is required.");
    });

    it("Should not be able to edit another user's task note", () => {
        Logger.log("Confirm edit button is not visible for another user's task note");
        taskNote
            .withNote("Other user task note")
            .hasDate(todayFormatted)
            .hasUser(rdoLondonUser.username)
            .noEditNoteLink();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
