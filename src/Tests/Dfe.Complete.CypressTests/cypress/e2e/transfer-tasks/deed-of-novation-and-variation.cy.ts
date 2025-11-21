import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
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

describe("Transfers tasks - Deed of novation and variation", () => {
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
        cy.visit(`projects/${projectId}/tasks/deed_of_novation_and_variation`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to an academy or trust are expected.")
            .clickDropdown("How to sign the deed of novation and variation")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'signed by the secretary of state' checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .tick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Deed of novation and variation")
            .selectTask("Deed of novation and variation");

        Logger.log("Unselect 'signed by the secretary of state' checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Deed of novation and variation")
            .selectTask("Deed of novation and variation");
        taskPage
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .isUnticked()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperTransfers.updateDeedOfNovationAndVariation(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Deed of novation and variation");

        TaskHelperTransfers.updateDeedOfNovationAndVariation(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Deed of novation and variation");

        TaskHelperTransfers.updateDeedOfNovationAndVariation(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Deed of novation and variation");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/deed_of_novation_and_variation`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
