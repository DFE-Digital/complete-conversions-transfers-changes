import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "trust_modification_order";

describe("Conversion tasks - Trust modification order", () => {
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
            .clickDropdown("When to use a trust modification order")
            .hasDropdownContent("A trust modification order is likely to be needed if the school is a:");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Sent to policy team for clearing")
            .tick()
            .hasCheckboxLabel("Sent to Academy Operations team mailbox for signing")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Trust modification order").selectTask("Trust modification order");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Sent to policy team for clearing")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Sent to Academy Operations team mailbox for signing")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Trust modification order").selectTask("Trust modification order");
        taskPage
            .hasCheckboxLabel("Sent to policy team for clearing")
            .isUnticked()
            .hasCheckboxLabel("Sent to Academy Operations team mailbox for signing")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateTrustModificationOrder(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Trust modification order");

        TaskHelperConversions.updateTrustModificationOrder(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Trust modification order");

        TaskHelperConversions.updateTrustModificationOrder(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Trust modification order");

        TaskHelperConversions.updateTrustModificationOrder(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Trust modification order");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
