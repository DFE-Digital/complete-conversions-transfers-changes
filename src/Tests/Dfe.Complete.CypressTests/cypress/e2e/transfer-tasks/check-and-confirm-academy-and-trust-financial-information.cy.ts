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
    urn: { value: urnPool.transferTasks.coquet },
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transferTasks.marden },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Transfers tasks - Check and confirm academy and trust financial information", () => {
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
        cy.visit(`projects/${projectId}/tasks/check_and_confirm_financial_information`);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Check and confirm academy and trust financial information")
            .selectTask("Check and confirm academy and trust financial information");

        Logger.log("Unselect 'not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Check and confirm academy and trust financial information")
            .selectTask("Check and confirm academy and trust financial information");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateCheckAndConfirmAcademyAndTrustFinancialInformation(taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Check and confirm academy and trust financial information");

        TaskHelper.updateCheckAndConfirmAcademyAndTrustFinancialInformation(
            taskId,
            ProjectType.Transfer,
            "notApplicable",
        );
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Check and confirm academy and trust financial information");

        TaskHelper.updateCheckAndConfirmAcademyAndTrustFinancialInformation(taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Check and confirm academy and trust financial information");

        TaskHelper.updateCheckAndConfirmAcademyAndTrustFinancialInformation(taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Check and confirm academy and trust financial information");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/check_and_confirm_financial_information`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
