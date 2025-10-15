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

describe("Conversion tasks - Land questionnaire", () => {
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
        cy.visit(`projects/${projectId}/tasks/land_questionnaire`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to clear a land questionnaire")
            .hasDropdownContent("You must check the school is using the right land questionnaire");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .tick()
            .hasCheckboxLabel("Cleared")
            .tick()
            .hasCheckboxLabel("Signed by solicitor")
            .tick()
            .hasCheckboxLabel("Saved in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusCompleted("Land questionnaire").selectTask("Land questionnaire");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Cleared")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Signed by solicitor")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the school's SharePoint folde")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Land questionnaire").selectTask("Land questionnaire");
        taskPage
            .hasCheckboxLabel("Received")
            .isUnticked()
            .hasCheckboxLabel("Cleared")
            .isUnticked()
            .hasCheckboxLabel("Signed by solicitor")
            .isUnticked()
            .hasCheckboxLabel("Saved in the school's SharePoint folde")
            .isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateLandQuestionnaire(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Land questionnaire");

        TaskHelper.updateLandQuestionnaire(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Land questionnaire");

        TaskHelper.updateLandQuestionnaire(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Land questionnaire");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/land_questionnaire`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
