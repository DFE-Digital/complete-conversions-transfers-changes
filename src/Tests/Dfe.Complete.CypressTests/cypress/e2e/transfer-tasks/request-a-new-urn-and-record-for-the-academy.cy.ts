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
    urn: urnPool.transferTasks.coquet,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
    createdByEmail: rdoLondonUser.email,
    createdByFirstName: rdoLondonUser.firstName,
    createdByLastName: rdoLondonUser.lastName,,
});
let otherUserProjectId: string;

describe("Transfer tasks - Request a new URN and record for the academy", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn).then((response) => {
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
        cy.visit(`projects/${projectId}/tasks/request_new_urn_and_record`);
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
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateRequestNewURNAndRecordForAcademy(taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Request a new URN and record for the academy");

        TaskHelper.updateRequestNewURNAndRecordForAcademy(taskId, ProjectType.Transfer, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Request a new URN and record for the academy");

        TaskHelper.updateRequestNewURNAndRecordForAcademy(taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Request a new URN and record for the academy");

        TaskHelper.updateRequestNewURNAndRecordForAcademy(taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Request a new URN and record for the academy");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/request_new_urn_and_record`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
