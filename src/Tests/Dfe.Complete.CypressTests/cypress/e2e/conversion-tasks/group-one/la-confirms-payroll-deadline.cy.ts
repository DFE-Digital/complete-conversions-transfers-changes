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
        const today = new Date();
        const dayTomorrow = String(today.getDate() + 1);
        const month = String(today.getMonth() + 1);
        const year = String(today.getFullYear());

        Logger.log("Input the date and save");
        taskPage.enterDate(dayTomorrow, month, year, "payroll-deadline").saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("LA confirms payroll deadline (LA)")
            .selectTask("LA confirms payroll deadline (LA)");

        Logger.log("Confirm date persists and clear date");
        taskPage.hasDate(dayTomorrow, month, year, "payroll-deadline").enterDate("", "", "", "payroll-deadline").saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("LA confirms payroll deadline (LA)")
            .selectTask("LA confirms payroll deadline (LA)");
        taskPage.hasDate("", "", "", "payroll-deadline");
    });

    it("should show validation errors", () => {
        Logger.log("Try to input a past date");
        taskPage
            .enterDate("15", "6", "2020", "payroll-deadline")
            .saveAndReturn()
            .hasLinkedValidationErrorForField('payroll-deadline.Day', "The payroll deadline must be in the future");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});