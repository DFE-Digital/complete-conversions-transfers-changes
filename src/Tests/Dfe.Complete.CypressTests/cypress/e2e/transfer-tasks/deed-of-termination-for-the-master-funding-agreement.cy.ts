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

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.coquet,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
});
let otherUserProjectId: string;

describe("Transfers tasks - Deed of termination for the master funding agreement", () => {
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
        cy.visit(`projects/${projectId}/tasks/deed_of_termination_for_the_master_funding_agreement`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Contact Financial Reporting team")
            .expandGuidance("How to contact the Financial Reporting team")
            .hasGuidance("Email the financial reporting team")
            .clickDropdown("How to sign the deed of termination")
            .hasDropdownContent(
                "The Secretary of State, or somebody with the authority to act on their behalf, must sign the deed of termination",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Deed of termination for the master funding agreement")
            .selectTask("Deed of termination for the master funding agreement");

        Logger.log("Unselect 'not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Deed of termination for the master funding agreement")
            .selectTask("Deed of termination for the master funding agreement");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateDeedOfTerminationMasterFundingAgreement(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Deed of termination for the master funding agreement");

        TaskHelper.updateDeedOfTerminationMasterFundingAgreement(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Deed of termination for the master funding agreement");

        TaskHelper.updateDeedOfTerminationMasterFundingAgreement(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Deed of termination for the master funding agreement");

        TaskHelper.updateDeedOfTerminationMasterFundingAgreement(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Deed of termination for the master funding agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/deed_of_termination_for_the_master_funding_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
