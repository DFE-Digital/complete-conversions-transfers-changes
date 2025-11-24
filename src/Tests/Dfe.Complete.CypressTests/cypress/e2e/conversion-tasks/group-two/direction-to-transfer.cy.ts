import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "direction_to_transfer";

describe("Conversion tasks - Direction to transfer", () => {
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
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("You need to compare the direction to transfer against the model documents");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage.hasCheckboxLabel("Received").tick().hasCheckboxLabel("Cleared").tick().saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Direction to transfer").selectTask("Direction to transfer");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Cleared")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Direction to transfer").selectTask("Direction to transfer");
        taskPage.hasCheckboxLabel("Received").isUnticked().hasCheckboxLabel("Cleared").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateDirectionToTransfer(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Direction to transfer");

        TaskHelperConversions.updateDirectionToTransfer(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Direction to transfer");

        TaskHelperConversions.updateDirectionToTransfer(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Direction to transfer");

        TaskHelperConversions.updateDirectionToTransfer(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Direction to transfer");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
