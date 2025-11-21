import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "check_and_confirm_financial_information";

describe("Transfer tasks - Check and confirm academy and trust financial information", () => {
    let setup: ReturnType<typeof TransferTasksTestSetup.getSetup>;

    before(() => {
        TransferTasksTestSetup.setupProjects();
        setup = TransferTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Check and confirm academy and trust financial information")
            .selectTask("Check and confirm academy and trust financial information");

        Logger.log("Unselect 'not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Check and confirm academy and trust financial information")
            .selectTask("Check and confirm academy and trust financial_information");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateCheckAndConfirmAcademyAndTrustFinancialInformation(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Check and confirm academy and trust financial information");

        TaskHelperTransfers.updateCheckAndConfirmAcademyAndTrustFinancialInformation(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Check and confirm academy and trust financial information");

        TaskHelperTransfers.updateCheckAndConfirmAcademyAndTrustFinancialInformation(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Check and confirm academy and trust financial information");

        TaskHelperTransfers.updateCheckAndConfirmAcademyAndTrustFinancialInformation(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Check and confirm academy and trust financial information");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/check_and_confirm_financial_information`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
