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
    urn: urnPool.transferTasks.coquet,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
});
let otherUserProjectId: string;

describe("Transfer tasks - Land consent letter", () => {
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
        cy.visit(`projects/${projectId}/tasks/land_consent_letter`);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Drafted")
            .tick()
            .hasCheckboxLabel("Sent to both trusts, their solicitors and the local authority")
            .tick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Land consent letter").selectTask("Land consent letter");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Drafted")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Sent to both trusts, their solicitors and the local authority")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Land consent letter").selectTask("Land consent letter");
        taskPage
            .hasCheckboxLabel("Drafted")
            .isUnticked()
            .hasCheckboxLabel("Sent to both trusts, their solicitors and the local authority")
            .isUnticked()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateLandConsentLetter(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Land consent letter");

        TaskHelper.updateLandConsentLetter(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Land consent letter");

        TaskHelper.updateLandConsentLetter(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Land consent letter");

        TaskHelper.updateLandConsentLetter(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Land consent letter");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/land_consent_letter`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
