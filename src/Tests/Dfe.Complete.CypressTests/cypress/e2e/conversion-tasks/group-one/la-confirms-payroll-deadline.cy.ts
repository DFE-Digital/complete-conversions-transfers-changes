import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";
import validationComponent from "cypress/pages/validationComponent";
import stakeholderKickOffTaskPage from "cypress/pages/projects/tasks/stakeholderKickOffTaskPage";

const taskPath = "la_confirms_payroll_deadline";
const stakeholderKickOffTaskPath = "stakeholder_kick_off";

describe("Conversion tasks - LA confirms payroll deadline", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;
    before(() => {
        cy.then(() => {
            ConversionTasksGroupOneSetup.setupProjectsWithoutTaskId();
            setup = ConversionTasksGroupOneSetup.getSetup();
        }).then(() => {
            return cy.wrap(null).should(() => {
                expect(setup.projectId, 'projectId should be set').to.not.be.empty;
            });
        }).then(() => {
            cy.login();
            cy.visit(`projects/${setup.projectId}/tasks/${stakeholderKickOffTaskPath}`);
            stakeholderKickOffTaskPage.enterSignificantDate(1, new Date().getFullYear() + 1).saveAndReturn();
            cy.visit(`projects/${setup.projectId}/tasks/${taskPath}`);
        });
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

    it.only("should show validation errors", () => {
        Logger.log("Try to input a past date");
        taskPage
            .enterDate("15", "6", "2020", "payroll-deadline")
            .saveAndReturn()

        validationComponent.hasLinkedValidationError("Payroll deadline must be in the future.");

        Logger.log("Try to input a date after the significant date");
        taskPage
            .enterDate("15", "6", String(new Date().getFullYear() + 10), "payroll-deadline")
            .saveAndReturn()

        validationComponent.hasLinkedValidationError("Payroll deadline must be before the significant date");

        Logger.log("Try to input an invalid date");
        taskPage
            .enterDate("15", "16", "2020", "payroll-deadline")
            .saveAndReturn()

        validationComponent.hasLinkedValidationError("Payroll deadline must be a real date");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});