import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "confirm_date_academy_transferred";

describe("Transfer tasks - Confirm the academy transfer date", () => {
    let setup: ReturnType<typeof TransferTasksTestSetup.getSetup>;

    before(() => {
        TransferTasksTestSetup.setupProjectsWithoutTaskId();
        setup = TransferTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Input the date and save");
        taskPage.enterDate("11", "1", "2025", "date-academy-transferred").saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the academy transfer date")
            .selectTask("Confirm the academy transfer date");

        Logger.log("Confirm date persists and clear date");
        taskPage
            .hasDate("11", "1", "2025", "date-academy-transferred")
            .enterDate("", "", "", "date-academy-transferred")
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the academy transfer date")
            .selectTask("Confirm the academy transfer date");
        taskPage.hasDate("", "", "", "date-academy-transferred");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/confirm_date_academy_transferred`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
