import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupTwoSetup } from "cypress/support/transferTasksSetup";

const taskPath = "request_new_urn_and_record";

describe("Transfer tasks - Request a new URN and record for the academy", () => {
    let setup: ReturnType<typeof TransferTasksGroupTwoSetup.getSetup>;

    before(() => {
        TransferTasksGroupTwoSetup.setupProjects();
        setup = TransferTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Complete the Fresh starts, transfers and changes for open academies form")
            .clickDropdown("Help with this form")
            .hasGuidance("There is guidance in a Microsoft Word document to help you complete the form");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Complete the Fresh starts, transfers and changes for open academies form")
            .tick()
            .hasCheckboxLabel("Give the new URN and LAESTAB information to the academy and incoming trust")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Request a new URN and record for the academy")
            .selectTask("Request a new URN and record for the academy");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Complete the Fresh starts, transfers and changes for open academies form")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Give the new URN and LAESTAB information to the academy and incoming trust")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Request a new URN and record for the academy")
            .selectTask("Request a new URN and record for the academy");
        taskPage
            .hasCheckboxLabel("Complete the Fresh starts, transfers and changes for open academies form")
            .isUnticked()
            .hasCheckboxLabel("Give the new URN and LAESTAB information to the academy and incoming trust")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateRequestNewURNAndRecordForAcademy(setup.taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Request a new URN and record for the academy");

        TaskHelperTransfers.updateRequestNewURNAndRecordForAcademy(setup.taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Request a new URN and record for the academy");

        TaskHelperTransfers.updateRequestNewURNAndRecordForAcademy(setup.taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Request a new URN and record for the academy");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/request_new_urn_and_record`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
