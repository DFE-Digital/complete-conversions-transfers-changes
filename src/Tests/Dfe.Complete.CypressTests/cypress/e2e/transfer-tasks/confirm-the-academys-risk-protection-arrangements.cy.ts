import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { Logger } from "cypress/common/logger";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

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

describe("Transfer tasks - Confirm the academy's risk protection arrangements", () => {
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
        cy.visit(`projects/${projectId}/tasks/rpa_policy`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("What do to if the academy changes its arrangements")
            .hasDropdownContent("The academy must be in an RPA or have commercially available insurance");
    });

    it("Should submit the form and persist selections", () => {
        Logger.log("Select the checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm if the academy's arrangements will continue or change")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the academy's risk protection arrangements")
            .selectTask("Confirm the academy's risk protection arrangements");

        Logger.log("Unselect the checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm if the academy's arrangements will continue or change")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the academy's risk protection arrangements")
            .selectTask("Confirm the academy's risk protection arrangements");
        taskPage.hasCheckboxLabel("Confirm if the academy's arrangements will continue or change").isUnticked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/rpa_policy`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
