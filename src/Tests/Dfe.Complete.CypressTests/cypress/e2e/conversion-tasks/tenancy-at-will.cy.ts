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

describe("Conversion tasks - Tenancy at will", () => {
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
        cy.visit(`projects/${projectId}/tasks/tenancy_at_will`);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all and save");
        taskPage
            .hasCheckboxLabel(
                "Email the school to ask if all relevant parties have agreed and signed the tenancy at will",
            )
            .tick()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed the tenancy at will",
            )
            .tick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusCompleted("Tenancy at will").selectTask("Tenancy at will");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel(
                "Email the school to ask if all relevant parties have agreed and signed the tenancy at will",
            )
            .isTicked()
            .untick()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed the tenancy at will",
            )
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Tenancy at will").selectTask("Tenancy at will");
        taskPage
            .hasCheckboxLabel(
                "Email the school to ask if all relevant parties have agreed and signed the tenancy at will",
            )
            .isUnticked()
            .hasCheckboxLabel(
                "Receive email from the school confirming all relevant parties have agreed and signed the tenancy at will",
            )
            .isUnticked()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperConversions.updateTenancyAtWill(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Tenancy at will");

        TaskHelperConversions.updateTenancyAtWill(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Tenancy at will");

        TaskHelperConversions.updateTenancyAtWill(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Tenancy at will");

        TaskHelperConversions.updateTenancyAtWill(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Tenancy at will");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/tenancy_at_will`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
