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

describe("Conversion tasks - Master funding agreement", () => {
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
        cy.visit(`projects/${projectId}/tasks/master_funding_agreement`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Help checking and updating the master funding agreement")
            .hasDropdownContent("Changes that personalise the model documents to a school or trust")
            .clickDropdown("How to sign the master funding agreement")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .tick()
            .hasCheckboxLabel("Saved in school and trust SharePoint folders")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Master funding agreement").selectTask("Master funding agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in school and trust SharePoint folders")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Master funding agreement").selectTask("Master funding agreement");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isUnticked()
            .hasCheckboxLabel("Saved in school and trust SharePoint folders")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperConversions.updateMasterFundingAgreement(taskId, ProjectType.Conversion, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Master funding agreement");

        TaskHelperConversions.updateMasterFundingAgreement(taskId, ProjectType.Conversion, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Master funding agreement");

        TaskHelperConversions.updateMasterFundingAgreement(taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Master funding agreement");

        TaskHelperConversions.updateMasterFundingAgreement(taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Master funding agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/master_funding_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
