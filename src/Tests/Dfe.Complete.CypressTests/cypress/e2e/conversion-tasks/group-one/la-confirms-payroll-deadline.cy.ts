import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "la_confirms_payroll_deadline";

describe("Conversion tasks - LA confirms payroll deadline", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjectsWithoutTaskId();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should be able to input a valid future date before the significant date", () => {
        Logger.log("Input the date and save");
        taskPage.enterDate("15", "6", "2025", "payroll-deadline").saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("LA confirms payroll deadline (LA)")
            .selectTask("LA confirms payroll deadline (LA)");

        Logger.log("Confirm date persists and clear date");
        taskPage.hasDate("15", "6", "2025", "payroll-deadline").enterDate("", "", "", "payroll-deadline").saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("LA confirms payroll deadline (LA)")
            .selectTask("LA confirms payroll deadline (LA)");
        taskPage.hasDate("", "", "", "payroll-deadline");
    });

    it("should show validation error for past dates", () => {
        Logger.log("Try to input a past date");
        taskPage.enterDate("15", "6", "2020", "payroll-deadline").saveTask().hasErrors();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});