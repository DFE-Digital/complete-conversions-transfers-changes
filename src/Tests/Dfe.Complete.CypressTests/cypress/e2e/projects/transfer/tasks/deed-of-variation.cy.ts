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

describe("Transfers tasks - Deed of variation", () => {
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
        cy.visit(`projects/${projectId}/tasks/deed_of_variation`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to an academy or trust are expected.")
            .clickDropdown("How to sign the deed of variation")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage.hasTaskStatusNotApplicable("Deed of variation").selectTask("Deed of variation");

        Logger.log("Unselect 'not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Deed of variation").selectTask("Deed of variation");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateDeedOfVariation(taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Deed of variation");

        TaskHelper.updateDeedOfVariation(taskId, ProjectType.Transfer, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Deed of variation");

        TaskHelper.updateDeedOfVariation(taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Deed of variation");

        TaskHelper.updateDeedOfVariation(taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Deed of variation");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/deed_of_variation`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
