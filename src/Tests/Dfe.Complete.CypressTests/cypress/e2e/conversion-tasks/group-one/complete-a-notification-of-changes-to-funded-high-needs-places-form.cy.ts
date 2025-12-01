import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "complete_notification_of_change";

describe("Conversion tasks - Complete a notification of changes to funded high needs places form", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("When to use the notification of change document")
            .hasDropdownContent("You will need to use the notification of changes document if the:")
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .expandGuidance("What to do if you do not have the document")
            .hasGuidance("Each year the academy openers team send out the notification of change document")
            .hasCheckboxLabel("Check the returned notification of change document")
            .expandGuidance("What to check for")
            .hasGuidance("You must make sure that the notification of change document includes");
    });

    it("Should submit the form and persist selections", () => {
        Logger.log("Select first 2 checkboxes and save");
        taskPage
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .tick()
            .hasCheckboxLabel("Check the returned notification of change document")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Complete a notification of changes to funded high needs places form")
            .selectTask("Complete a notification of changes to funded high needs places form");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Check the returned notification of change document")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Complete a notification of changes to funded high needs places form")
            .selectTask("Complete a notification of changes to funded high needs places form");
        taskPage
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .isUnticked()
            .hasCheckboxLabel("Check the returned notification of change document")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateCompleteNotificationOfChange(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Complete a notification of changes to funded high needs places form");

        TaskHelperConversions.updateCompleteNotificationOfChange(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Complete a notification of changes to funded high needs places form");

        TaskHelperConversions.updateCompleteNotificationOfChange(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Complete a notification of changes to funded high needs places form");

        TaskHelperConversions.updateCompleteNotificationOfChange(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Complete a notification of changes to funded high needs places form");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
