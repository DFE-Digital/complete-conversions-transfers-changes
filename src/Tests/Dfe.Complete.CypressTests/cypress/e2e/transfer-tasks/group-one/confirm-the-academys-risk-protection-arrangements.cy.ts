import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";

const taskPath = "rpa_policy";

describe("Transfer tasks - Confirm the academy's risk protection arrangements", () => {
    let setup: ReturnType<typeof TransferTasksGroupOneSetup.getSetup>;

    before(() => {
        TransferTasksGroupOneSetup.setupProjects();
        setup = TransferTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("What do to if the academy changes its arrangements")
            .hasDropdownContent("The academy must be in an RPA or have commercially available insurance");
    });

    it("Should submit the form and persist selections", () => {
        Logger.log("Select the checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm if the academy's arrangements will continue or change")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the academy's risk protection arrangements")
            .selectTask("Confirm the academy's risk protection arrangements");

        Logger.log("Unselect the checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm if the academy's arrangements will continue or change")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the academy's risk protection arrangements")
            .selectTask("Confirm the academy's risk protection arrangements");
        taskPage.hasCheckboxLabel("Confirm if the academy's arrangements will continue or change").isUnticked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/rpa_policy`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
