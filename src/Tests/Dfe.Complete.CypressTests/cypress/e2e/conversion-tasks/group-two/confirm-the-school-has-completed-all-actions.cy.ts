import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "school_completed";

describe("Conversion tasks - Confirm school has completed all actions", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjects();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Send the final email checklist to the main contact")
            .hasCheckboxLabel("Save response in school’s Sharepoint folder")
            .expandGuidance("What to do if the school has not completed the tasks")
            .hasGuidance("Depending on the scale of the outstanding tasks");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Send the final email checklist to the main contact")
            .tick()
            .hasCheckboxLabel("Save response in school’s Sharepoint folder")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm school has completed all actions")
            .selectTask("Confirm school has completed all actions");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Send the final email checklist to the main contact")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save response in school’s Sharepoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm school has completed all actions")
            .selectTask("Confirm school has completed all actions");
        taskPage
            .hasCheckboxLabel("Send the final email checklist to the main contact")
            .isUnticked()
            .hasCheckboxLabel("Save response in school’s Sharepoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateConfirmSchoolHasCompletedAllActions(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm school has completed all actions");

        TaskHelperConversions.updateConfirmSchoolHasCompletedAllActions(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Confirm school has completed all actions");

        TaskHelperConversions.updateConfirmSchoolHasCompletedAllActions(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm school has completed all actions");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
