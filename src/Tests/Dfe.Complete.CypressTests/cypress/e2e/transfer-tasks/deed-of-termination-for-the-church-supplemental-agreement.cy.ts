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
    urn: { value: urnPool.transferTasks.coquet },
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transferTasks.marden },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Transfers tasks - Deed of termination for the church supplemental agreement", () => {
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
        cy.visit(`projects/${projectId}/tasks/deed_termination_church_agreement`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Other ways to deal with a church supplemental agreement")
            .hasDropdownContent("Solicitors may want to transfer a church supplemental agreement to the new trust")
            .clickDropdown("Using a deed of termination to transfer a church supplemental agreement")
            .hasDropdownContent("There is no model document for using a deed of termination to transfer")
            .clickDropdown("How to sign the deed of termination")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Deed of termination for the church supplemental agreement")
            .selectTask("Deed of termination for the church supplemental agreement");

        Logger.log("Unselect 'not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Deed of termination for the church supplemental agreement")
            .selectTask("Deed of termination for the church supplemental agreement");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateDeedOfTerminationChurchSupplementalAgreement(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Deed of termination for the church supplemental agreement");

        TaskHelper.updateDeedOfTerminationChurchSupplementalAgreement(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Deed of termination for the church supplemental agreement");

        TaskHelper.updateDeedOfTerminationChurchSupplementalAgreement(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Deed of termination for the church supplemental agreement");

        TaskHelper.updateDeedOfTerminationChurchSupplementalAgreement(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Deed of termination for the church supplemental agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/deed_termination_church_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
