import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "confirm_date_academy_opened";

describe("Conversion tasks - Confirm date academy opened", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjectsWithoutTaskId();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should be able to input a valid past or future date", () => {
        Logger.log("Input the date and save");
        taskPage.enterDate("10", "10", "2024", "opened-date").saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm date academy opened")
            .selectTask("Confirm date academy opened");

        Logger.log("Confirm date persists and clear date");
        taskPage.hasDate("10", "10", "2024", "opened-date").enterDate("", "", "", "opened-date").saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm date academy opened")
            .selectTask("Confirm date academy opened");
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
