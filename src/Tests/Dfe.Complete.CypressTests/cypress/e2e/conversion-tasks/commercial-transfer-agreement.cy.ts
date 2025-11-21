import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
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

describe("Conversion tasks - Commercial transfer agreement", () => {
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
        cy.visit(`projects/${projectId}/tasks/commercial_transfer_agreement`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to check and assure the commercial transfer agreement")
            .hasDropdownContent(
                "You can read guidance about how use check and assure the agreement (opens in new tab) on SharePoint.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Check solicitor responses to assurance questions")
            .tick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Check solicitor responses to assurance questions")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");
        taskPage
            .hasCheckboxLabel("Check solicitor responses to assurance questions")
            .isUnticked()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperConversions.updateCommercialTransferAgreement(taskId, ProjectType.Conversion, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Commercial transfer agreement");

        TaskHelperConversions.updateCommercialTransferAgreement(taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Commercial transfer agreement");

        TaskHelperConversions.updateCommercialTransferAgreement(taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Commercial transfer agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/commercial_transfer_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
