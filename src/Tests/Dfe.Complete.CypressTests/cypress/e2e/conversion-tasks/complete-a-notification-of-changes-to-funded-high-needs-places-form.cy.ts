import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
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

describe("Conversion tasks - Complete a notification of changes to funded high needs places form", () => {
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
        cy.visit(`projects/${projectId}/tasks/complete_notification_of_change`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("When to use the notification of change document")
            .hasDropdownContent("You will need to use the notification of changes document if the:")
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .expandGuidance("What to do if you do not have the document")
            .hasGuidance("Each year the academy openers team send out the notification of change document")
            .hasCheckboxLabel("Check the returned notification of change document")
            .expandGuidance("What to check for")
            .hasGuidance("You must make sure that the notification of change document includes");
    });

    it("Should submit the form and persist selections", () => {
        Logger.log("Select first 2 checkboxes and save");
        taskPage
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .tick()
            .hasCheckboxLabel("Check the returned notification of change document")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Complete a notification of changes to funded high needs places form")
            .selectTask("Complete a notification of changes to funded high needs places form");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Check the returned notification of change document")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Complete a notification of changes to funded high needs places form")
            .selectTask("Complete a notification of changes to funded high needs places form");
        taskPage
            .hasCheckboxLabel("Tell the local authority to complete the notification of change document")
            .isUnticked()
            .hasCheckboxLabel("Check the returned notification of change document")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperConversions.updateCompleteNotificationOfChange(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Complete a notification of changes to funded high needs places form");

        TaskHelperConversions.updateCompleteNotificationOfChange(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Complete a notification of changes to funded high needs places form");

        TaskHelperConversions.updateCompleteNotificationOfChange(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Complete a notification of changes to funded high needs places form");

        TaskHelperConversions.updateCompleteNotificationOfChange(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Complete a notification of changes to funded high needs places form");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/complete_notification_of_change`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
