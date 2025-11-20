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
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
});
let otherUserProjectId: string;

describe("Conversion tasks - Process conversion support grant", () => {
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
        cy.visit(`projects/${projectId}/tasks/conversion_grant`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to process a conversion support grant")
            .hasDropdownContent("Check the grant claim form is ready")
            .hasCheckboxLabel("Check the school or trust have a vendor account")
            .expandGuidance("How to set up a vendor account")
            .hasGuidance("The school or trust receiving the conversion support grant must have a vendor account.")
            .hasCheckboxLabel("Receive, check and save the grant claim form in the school's SharePoint folder")
            .expandGuidance("How to check and save the form")
            .hasGuidance("The school or trust must send you the grant claim form.");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Send the grant information and vendor account details to the payments team")
            .tick()
            .hasCheckboxLabel("Tell the school or trust the grant information has been sent")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Process conversion support grant")
            .selectTask("Process conversion support grant");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Send the grant information and vendor account details to the payments team")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Tell the school or trust the grant information has been sent")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Process conversion support grant")
            .selectTask("Process conversion support grant");
        taskPage
            .hasCheckboxLabel("Send the grant information and vendor account details to the payments team")
            .isUnticked()
            .hasCheckboxLabel("Tell the school or trust the grant information has been sent")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateProcessConversionSupportGrant(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Process conversion support grant");

        TaskHelper.updateProcessConversionSupportGrant(taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Process conversion support grant");

        TaskHelper.updateProcessConversionSupportGrant(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Process conversion support grant");

        TaskHelper.updateProcessConversionSupportGrant(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Process conversion support grant");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/conversion_grant`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
