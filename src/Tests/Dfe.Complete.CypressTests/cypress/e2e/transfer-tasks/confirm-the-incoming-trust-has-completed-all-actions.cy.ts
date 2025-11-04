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

describe("Transfer tasks - Confirm the incoming trust has completed all actions", () => {
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
        cy.visit(`projects/${projectId}/tasks/confirm_incoming_trust_has_completed_all_actions`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Email the primary contact for the incoming trust")
            .clickDropdown("How to structure your email")
            .hasGuidance("You should ask them to check and confirm they've completed all required tasks")
            .hasCheckboxLabel(
                "Save the responses to the checklist in the academy and incoming trust SharePoint folders",
            )
            .clickDropdown("What to do if the incoming trust has not completed the tasks")
            .hasGuidance("If the transfer date needs to be moved, you must:");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the primary contact for the incoming trust")
            .tick()
            .hasCheckboxLabel(
                "Save the responses to the checklist in the academy and incoming trust SharePoint folders",
            )
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the incoming trust has completed all actions")
            .selectTask("Confirm the incoming trust has completed all actions");

        Logger.log("Unselect the all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the primary contact for the incoming trust")
            .isTicked()
            .untick()
            .hasCheckboxLabel(
                "Save the responses to the checklist in the academy and incoming trust SharePoint folders",
            )
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the incoming trust has completed all actions")
            .selectTask("Confirm the incoming trust has completed all actions");
        taskPage
            .hasCheckboxLabel("Email the primary contact for the incoming trust")
            .isUnticked()
            .hasCheckboxLabel(
                "Save the responses to the checklist in the academy and incoming trust SharePoint folders",
            )
            .isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateIncomingTrustHasCompletedAllActions(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm the incoming trust has completed all actions");

        TaskHelper.updateIncomingTrustHasCompletedAllActions(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Confirm the incoming trust has completed all actions");

        TaskHelper.updateIncomingTrustHasCompletedAllActions(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the incoming trust has completed all actions");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/confirm_incoming_trust_has_completed_all_actions`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
