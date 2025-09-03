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

const project = ProjectBuilder.createTransferProjectRequest();
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Transfer - Handover with regional delivery officer", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${otherUserProject.urn.value}`);
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
        cy.visit(`projects/${projectId}/tasks/handover`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Review the project information and carry out research")
            .expandGuidance("What to check for")
            .hasGuidance("Things to check during your background research include:")
            .hasCheckboxLabel("Make notes and write questions to ask the regional delivery officer")
            .expandGuidance("What to check for")
            .hasGuidance("Note down things you want to ask");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Attend handover meeting' checkbox and save");
        taskPage.hasCheckboxLabel("Attend handover meeting with regional delivery officer").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Handover with regional delivery officer")
            .selectTask("Handover with regional delivery officer");

        Logger.log("Unselect the 'Attend handover meeting'checkbox and save");
        taskPage
            .hasCheckboxLabel("Attend handover meeting with regional delivery officer")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Handover with regional delivery officer")
            .selectTask("Handover with regional delivery officer");
        taskPage.hasCheckboxLabel("Attend handover meeting with regional delivery officer").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateHandoverWithDeliveryOfficer(taskId, ProjectType.Transfer, "notStarted");
        taskListPage.hasTaskStatusNotStarted("Handover with regional delivery officer");

        TaskHelper.updateHandoverWithDeliveryOfficer(taskId, ProjectType.Transfer, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Handover with regional delivery officer");

        TaskHelper.updateHandoverWithDeliveryOfficer(taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Handover with regional delivery officer");

        TaskHelper.updateHandoverWithDeliveryOfficer(taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Handover with regional delivery officer");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/handover`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
