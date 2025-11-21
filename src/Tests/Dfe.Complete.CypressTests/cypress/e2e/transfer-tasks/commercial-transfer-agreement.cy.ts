import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import projectRemover from "cypress/api/projectRemover";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.coquet,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
});
let otherUserProjectId: string;

describe("Conversion tasks - Commercial transfer agreement", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn).then((response) => {
                taskId = response.body.tasksDataId.value;
            });
        });
        projectApi.createAndUpdateMatTransferProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/commercial_transfer_agreement`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to check and assure the commercial transfer agreement")
            .hasDropdownContent(
                "You can read guidance about how use check and assure the agreement (opens in new tab) on SharePoint.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'Confirm commercial transfer agreement is agreed' and save");
        taskPage.hasCheckboxLabel("Confirm commercial transfer agreement is agreed").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");

        Logger.log("Unselect 'Confirm commercial transfer agreement is agreed' and save");
        taskPage
            .hasCheckboxLabel("Confirm commercial transfer agreement is agreed")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");
        taskPage.hasCheckboxLabel("Confirm commercial transfer agreement is agreed").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperTransfers.updateCommercialTransferAgreement(taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Commercial transfer agreement");

        TaskHelperTransfers.updateCommercialTransferAgreement(taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Commercial transfer agreement");

        TaskHelperTransfers.updateCommercialTransferAgreement(taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Commercial transfer agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/commercial_transfer_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
