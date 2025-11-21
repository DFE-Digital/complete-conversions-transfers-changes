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

describe("Conversion tasks - Check accuracy of high needs places information", () => {
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
        cy.visit(`projects/${projectId}/tasks/check_accuracy_of_higher_needs`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Confirm if the academy will have high needs places ESFA must fund")
            .expandGuidance("Help checking and confirming high needs places")
            .hasGuidance("Understanding differences in high needs places information")
            .hasCheckboxLabel(
                "Confirm the section 251 spreadsheet shows the number of high needs places ESFA must fund",
            )
            .expandGuidance("Help checking the 251 spreadsheet")
            .hasGuidance("The section 251 spreadsheet shows how many high needs places the local authority funds.");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Confirm if the academy will have high needs places ESFA must fund")
            .tick()
            .hasCheckboxLabel(
                "Confirm the section 251 spreadsheet shows the number of high needs places ESFA must fund",
            )
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Check accuracy of high needs places information")
            .selectTask("Check accuracy of high needs places information");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Confirm if the academy will have high needs places ESFA must fund")
            .isTicked()
            .untick()
            .hasCheckboxLabel(
                "Confirm the section 251 spreadsheet shows the number of high needs places ESFA must fund",
            )
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Check accuracy of high needs places information")
            .selectTask("Check accuracy of high needs places information");
        taskPage
            .hasCheckboxLabel("Confirm if the academy will have high needs places ESFA must fund")
            .isUnticked()
            .hasCheckboxLabel(
                "Confirm the section 251 spreadsheet shows the number of high needs places ESFA must fund",
            )
            .isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelperConversions.updateCheckAccuracyOfHigherNeeds(taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Check accuracy of high needs places information");

        TaskHelperConversions.updateCheckAccuracyOfHigherNeeds(taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Check accuracy of high needs places information");

        TaskHelperConversions.updateCheckAccuracyOfHigherNeeds(taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Check accuracy of high needs places information");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/check_accuracy_of_higher_needs`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
