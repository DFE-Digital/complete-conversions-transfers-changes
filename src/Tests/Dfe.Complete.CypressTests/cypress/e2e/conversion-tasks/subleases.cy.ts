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

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
    createdByEmail: rdoLondonUser.email,
    createdByFirstName: rdoLondonUser.firstName,
    createdByLastName: rdoLondonUser.lastName,,
});
let otherUserProjectId: string;

describe("Conversion tasks - Subleases", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn).then((response) => {
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
        cy.visit(`projects/${projectId}/tasks/subleases`);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the school to ask if all relevant parties have agreed and signed any subleases")
            .tick()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed any subleases",
            )
            .tick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Subleases").selectTask("Subleases");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Email the school to ask if all relevant parties have agreed and signed any subleases")
            .isTicked()
            .untick()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed any subleases",
            )
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Subleases").selectTask("Subleases");
        taskPage
            .hasCheckboxLabel("Email the school to ask if all relevant parties have agreed and signed any subleases")
            .isUnticked()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed any subleases",
            )
            .isUnticked()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateSubleases(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Subleases");

        TaskHelper.updateSubleases(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Subleases");

        TaskHelper.updateSubleases(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Subleases");

        TaskHelper.updateSubleases(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Subleases");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/subleases`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
