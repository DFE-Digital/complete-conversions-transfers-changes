import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "confirm_date_academy_opened";

describe("Conversion tasks - Confirm the academy open date", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupProjectsWithoutTaskId();
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should be able to input a valid past or future date", () => {
        Logger.log("Input the date and save");
        taskPage.enterDate("10", "10", "2024", "opened-date").saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the academy open date")
            .selectTask("Confirm the academy open date");

        Logger.log("Confirm date persists and clear date");
        taskPage.hasDate("10", "10", "2024", "opened-date").enterDate("", "", "", "opened-date").saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the academy open date")
            .selectTask("Confirm the academy open date");
        taskPage.hasDate("", "", "", "opened-date");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
