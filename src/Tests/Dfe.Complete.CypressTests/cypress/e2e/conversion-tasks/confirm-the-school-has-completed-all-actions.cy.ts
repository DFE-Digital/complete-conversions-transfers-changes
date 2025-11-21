import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "school_completed";

describe("Conversion tasks - Confirm the school has completed all actions", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupProjects();
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Email the main contact for the conversion")
            .expandGuidance("How to structure your email")
            .hasGuidance("You should ask them to check and confirm they've completed all tasks")
            .hasCheckboxLabel("Save the responses to the checklist in the school's SharePoint folder")
            .expandGuidance("What to do if the school has not completed the tasks")
            .hasGuidance("Depending on the scale of the outstanding tasks");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the main contact for the conversion")
            .tick()
            .hasCheckboxLabel("Save the responses to the checklist in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the school has completed all actions")
            .selectTask("Confirm the school has completed all actions");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the main contact for the conversion")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save the responses to the checklist in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the school has completed all actions")
            .selectTask("Confirm the school has completed all actions");
        taskPage
            .hasCheckboxLabel("Email the main contact for the conversion")
            .isUnticked()
            .hasCheckboxLabel("Save the responses to the checklist in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateConfirmSchoolHasCompletedAllActions(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm the school has completed all actions");

        TaskHelperConversions.updateConfirmSchoolHasCompletedAllActions(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Confirm the school has completed all actions");

        TaskHelperConversions.updateConfirmSchoolHasCompletedAllActions(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the school has completed all actions");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
