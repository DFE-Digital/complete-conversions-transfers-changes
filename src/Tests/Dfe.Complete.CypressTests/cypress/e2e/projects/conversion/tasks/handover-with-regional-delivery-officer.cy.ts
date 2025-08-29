import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskApi, { ProjectType } from "cypress/api/taskApi";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Conversion - Handover with regional delivery officer", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${otherUserProject.urn.value}`);
        projectApi.createMatConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn.value).then((response) => {
                taskId = response.body.tasksDataId.value;
            });
        });
        projectApi.createMatConversionProject(otherUserProject).then((createResponse) => {
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
            .hasCheckboxLabel("Review the project information, check the documents and carry out research")
            .expandGuidance("What to check for")
            .hasGuidance("You should check existing project documents, including:")
            .hasCheckboxLabel("Make notes and write questions to ask the regional delivery officer")
            .expandGuidance("What to make notes about")
            .hasGuidance("Note down things you want to ask");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the Not applicable checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Handover with regional delivery officer")
            .selectTask("Handover with regional delivery officer");

        Logger.log("Unselect the Not applicable checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Handover with regional delivery officer")
            .selectTask("Handover with regional delivery officer");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        taskApi.updateHandoverWithDeliveryOfficerTask(taskId, ProjectType.Conversion);
        taskListPage.hasTaskStatusNotStarted("Handover with regional delivery officer");

        taskApi.updateHandoverWithDeliveryOfficerTask(taskId, ProjectType.Conversion, true);
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Handover with regional delivery officer");

        taskApi.updateHandoverWithDeliveryOfficerTask(taskId, ProjectType.Conversion, false, true);
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Handover with regional delivery officer");

        taskApi.updateHandoverWithDeliveryOfficerTask(taskId, ProjectType.Conversion, false, true, true, true);
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
