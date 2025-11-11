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
    urn: { value: urnPool.conversionTasks.spen },
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: urnPool.conversionTasks.grylls },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Conversion tasks - 125 year lease", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectRemover.removeProjectIfItExists(otherUserProject.urn.value);
        projectApi.createConversionProject(project).then((createResponse) => {
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
        cy.visit(`projects/${projectId}/tasks/one_hundred_and_twenty_five_year_lease`);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel(
                "Email the solicitors to ask if all relevant parties have agreed and signed the 125 year lease",
            )
            .tick()
            .hasCheckboxLabel(
                "Receive email from the solicitors confirming all relevant parties have agreed and signed the 125 year lease",
            )
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("125 year lease").selectTask("125 year lease");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel(
                "Email the solicitors to ask if all relevant parties have agreed and signed the 125 year lease",
            )
            .isTicked()
            .untick()
            .hasCheckboxLabel(
                "Receive email from the solicitors confirming all relevant parties have agreed and signed the 125 year lease",
            )
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("125 year lease").selectTask("125 year lease");
        taskPage
            .hasCheckboxLabel(
                "Email the solicitors to ask if all relevant parties have agreed and signed the 125 year lease",
            )
            .isUnticked()
            .hasCheckboxLabel(
                "Receive email from the solicitors confirming all relevant parties have agreed and signed the 125 year lease",
            )
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateOneHundredAndTwentyFiveYearLease(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("125 year lease");

        TaskHelper.updateOneHundredAndTwentyFiveYearLease(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("125 year lease");

        TaskHelper.updateOneHundredAndTwentyFiveYearLease(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("125 year lease");

        TaskHelper.updateOneHundredAndTwentyFiveYearLease(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("125 year lease");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/one_hundred_and_twenty_five_year_lease`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
