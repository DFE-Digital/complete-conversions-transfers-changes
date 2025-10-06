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

describe("Conversion tasks - Articles of association", () => {
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
        cy.visit(`projects/${projectId}/tasks/articles_of_association`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Partially updating articles of association")
            .hasDropdownContent(
                "Trusts do not have to adopt the latest version of the articles entirely. Although this is DfE's preferred option.",
            )
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to a school or trust");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Received' checkbox and save");
        taskPage.hasCheckboxLabel("Received").tick().saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Articles of association").selectTask("Articles of association");

        Logger.log("Unselect the 'Received' checkbox and save");
        taskPage.hasCheckboxLabel("Received").isTicked().untick().saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Articles of association").selectTask("Articles of association");
        taskPage.hasCheckboxLabel("Received").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateArticleOfAssociation(taskId, ProjectType.Conversion, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Articles of association");

        TaskHelper.updateArticleOfAssociation(taskId, ProjectType.Conversion, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Articles of association");

        TaskHelper.updateArticleOfAssociation(taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Articles of association");

        TaskHelper.updateArticleOfAssociation(taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Articles of association");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/handover`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
