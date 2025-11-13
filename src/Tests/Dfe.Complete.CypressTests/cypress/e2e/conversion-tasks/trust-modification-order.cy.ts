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
});
let otherUserProjectId: string;

describe("Conversion tasks - Trust modification order", () => {
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
        cy.visit(`projects/${projectId}/tasks/trust_modification_order`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("When to use a trust modification order")
            .hasDropdownContent("A trust modification order is likely to be needed if the school is a:");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Sent to policy team for clearing")
            .tick()
            .hasCheckboxLabel("Sent to Academy Operations team mailbox for signing")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Trust modification order").selectTask("Trust modification order");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Sent to policy team for clearing")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Sent to Academy Operations team mailbox for signing")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Trust modification order").selectTask("Trust modification order");
        taskPage
            .hasCheckboxLabel("Sent to policy team for clearing")
            .isUnticked()
            .hasCheckboxLabel("Sent to Academy Operations team mailbox for signing")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateTrustModificationOrder(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Trust modification order");

        TaskHelper.updateTrustModificationOrder(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Trust modification order");

        TaskHelper.updateTrustModificationOrder(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Trust modification order");

        TaskHelper.updateTrustModificationOrder(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Trust modification order");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/trust_modification_order`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
