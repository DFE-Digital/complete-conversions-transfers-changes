import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "one_hundred_and_twenty_five_year_lease";

describe("Conversion tasks - 125 year lease", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel(
                "Email the solicitors to ask if all relevant parties have agreed and signed the 125 year lease",
            )
            .tick()
            .hasCheckboxLabel(
                "Receive email from the solicitors confirming all relevant parties have agreed and signed the 125 year lease",
            )
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("125 year lease").selectTask("125 year lease");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel(
                "Email the solicitors to ask if all relevant parties have agreed and signed the 125 year lease",
            )
            .isTicked()
            .untick()
            .hasCheckboxLabel(
                "Receive email from the solicitors confirming all relevant parties have agreed and signed the 125 year lease",
            )
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("125 year lease").selectTask("125 year lease");
        taskPage
            .hasCheckboxLabel(
                "Email the solicitors to ask if all relevant parties have agreed and signed the 125 year lease",
            )
            .isUnticked()
            .hasCheckboxLabel(
                "Receive email from the solicitors confirming all relevant parties have agreed and signed the 125 year lease",
            )
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateOneHundredAndTwentyFiveYearLease(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("125 year lease");

        TaskHelperConversions.updateOneHundredAndTwentyFiveYearLease(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("125 year lease");

        TaskHelperConversions.updateOneHundredAndTwentyFiveYearLease(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("125 year lease");

        TaskHelperConversions.updateOneHundredAndTwentyFiveYearLease(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("125 year lease");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
