import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
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

describe("Transfer tasks - Redact and send documents", () => {
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
        cy.visit(`projects/${projectId}/tasks/redact_and_send_documents`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Redact all relevant documents")
            .expandGuidance("Help redacting the documents")
            .hasGuidance("You need to create a redacted version of each document")
            .hasGuidance("deed of novation and variation")
            .hasGuidance("deed of termination");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select first checkbox and save");
        taskPage
            .hasCheckboxLabel("Send relevant redacted documents to ESFA (Education and Skills Funding Agency)")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Redact and send documents").selectTask("Redact and send documents");

        Logger.log("Unselect same checkbox and save");
        taskPage
            .hasCheckboxLabel("Send relevant redacted documents to ESFA (Education and Skills Funding Agency)")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents").selectTask("Redact and send documents");
        taskPage
            .hasCheckboxLabel("Send relevant redacted documents to ESFA (Education and Skills Funding Agency)")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateRedactAndSendDocuments(taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents");

        TaskHelper.updateRedactAndSendDocuments(taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Redact and send documents");

        TaskHelper.updateRedactAndSendDocuments(taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Redact and send documents");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/redact_and_send_documents`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
