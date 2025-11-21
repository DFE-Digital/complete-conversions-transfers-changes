import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
});
let otherUserProjectId: string;

describe("Conversion tasks - Redact and send documents", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn).then((response) => {
                taskId = response.body.tasksDataId.value;
            });
        });
        projectApi.createAndUpdateMatConversionProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/redact_and_send`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Redact all relevant documents")
            .expandGuidance("Help redacting the documents")
            .hasGuidance("You need to create a redacted version of each applicable document");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select first checkbox and save");
        taskPage.hasCheckboxLabel("Redact all relevant documents").tick().saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Redact and send documents").selectTask("Redact and send documents");

        Logger.log("Unselect same checkbox and save");
        taskPage.hasCheckboxLabel("Redact all relevant documents").isTicked().untick().saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents").selectTask("Redact and send documents");
        taskPage.hasCheckboxLabel("Redact all relevant documents").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperConversions.updateRedactAndSendDocuments(taskId, ProjectType.Conversion, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Redact and send documents");

        TaskHelperConversions.updateRedactAndSendDocuments(taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Redact and send documents");

        TaskHelperConversions.updateRedactAndSendDocuments(taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Redact and send documents");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/redact_and_send`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
