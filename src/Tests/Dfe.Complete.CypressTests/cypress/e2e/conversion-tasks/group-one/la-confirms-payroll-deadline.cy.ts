import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";
import validationComponent from "cypress/pages/validationComponent";
import taskHelperConversions from "cypress/api/taskHelperConversions";
import { getSignificantDateString } from "cypress/support/formatDate";

const taskPath = "la_confirms_payroll_deadline";

const today = new Date();
const dayTomorrow = String(today.getDate() + 1);
const month = String(today.getMonth() + 1);
const year = String(today.getFullYear());
const yearTwoYearsFromNow = String(today.getFullYear() + 2);
const significantDateNextYear = getSignificantDateString(12);

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
            taskHelperConversions.updateExternalStakeholderKickOff(setup.projectId, "completed", significantDateNextYear);
        });
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should be able to input a valid future date before the significant date", () => {
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
        Logger.log("Try to input a date after the significant date");
        taskPage
            .enterDate("15", "6", yearTwoYearsFromNow, "payroll-deadline")
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