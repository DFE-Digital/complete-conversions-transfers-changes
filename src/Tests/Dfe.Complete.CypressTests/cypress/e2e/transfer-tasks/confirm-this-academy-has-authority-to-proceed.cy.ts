import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelper from "cypress/api/taskHelper";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.transferTasks.coquet },
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transferTasks.marden },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Transfer tasks - Confirm this transfer has authority to proceed", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectRemover.removeProjectIfItExists(otherUserProject.urn.value);
        projectApi.createTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn.value).then((response) => {
                taskId = response.body.tasksDataId.value;
            });
        });
        projectApi.createMatTransferProject(otherUserProject).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/conditions_met`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to check the transfer has authority to proceed")
            .hasDropdownContent("the academy have confirmed all of their actions are complete")
            .clickDropdown("What to do if the transfer does not have authority to proceed")
            .hasDropdownContent(
                "You must agree a new transfer date with all stakeholders, then change the transfer date for this project.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Baseline spreadsheet approved' checkbox and save");
        taskPage.hasCheckboxLabel("Baseline spreadsheet approved").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Confirm this transfer has authority to proceed")
            .selectTask("Confirm this transfer has authority to proceed");

        Logger.log("Unselect the 'Baseline spreadsheet approved' checkbox and save");
        taskPage.hasCheckboxLabel("Baseline spreadsheet approved").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm this transfer has authority to proceed")
            .selectTask("Confirm this transfer has authority to proceed");
        taskPage.hasCheckboxLabel("Baseline spreadsheet approved").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateConfirmTransferHasAuthorityToProceed(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm this transfer has authority to proceed");

        TaskHelper.updateConfirmTransferHasAuthorityToProceed(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Confirm this transfer has authority to proceed");

        TaskHelper.updateConfirmTransferHasAuthorityToProceed(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm this transfer has authority to proceed");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/conditions_met`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
