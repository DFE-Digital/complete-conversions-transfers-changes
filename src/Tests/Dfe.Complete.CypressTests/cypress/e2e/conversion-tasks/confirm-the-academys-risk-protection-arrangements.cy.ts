import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { Logger } from "cypress/common/logger";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import TaskHelper from "cypress/api/taskHelper";
import { ProjectType } from "cypress/api/taskApi";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
let taskId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
    createdByEmail: rdoLondonUser.email,
    createdByFirstName: rdoLondonUser.firstName,
    createdByLastName: rdoLondonUser.lastName,,
});
let otherUserProjectId: string;

describe("Conversion tasks - Confirm the academy's risk protection arrangements", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn).then((response) => {
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
        cy.visit(`projects/${projectId}/tasks/risk_protection_arrangement`);
    });

    it("Should submit the form and persist selections", () => {
        Logger.log("Select the 'No, buying commercial insurance' option with a reason and save");
        taskPage
            .hasCheckboxLabel("No, buying commercial insurance")
            .tick()
            .type("Reason for buying commercial insurance")
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the academy's risk protection arrangements")
            .selectTask("Confirm the academy's risk protection arrangements");

        Logger.log("Change selection");
        taskPage
            .hasCheckboxLabel("No, buying commercial insurance")
            .isTicked()
            .hasCheckboxLabel("Yes, joining standard RPA")
            .isUnticked()
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the academy's risk protection arrangements")
            .selectTask("Confirm the academy's risk protection arrangements");
        taskPage.hasCheckboxLabel("Yes, joining standard RPA").isTicked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateConfirmAcademyRiskProtectionArrangements(taskId, ProjectType.Conversion);
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm the academy's risk protection arrangements");

        TaskHelper.updateConfirmAcademyRiskProtectionArrangements(
            taskId,
            ProjectType.Conversion,
            undefined,
            "Standard",
        );
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the academy's risk protection arrangements");

        TaskHelper.updateConfirmAcademyRiskProtectionArrangements(
            taskId,
            ProjectType.Conversion,
            undefined,
            "ChurchOrTrust",
        );
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the academy's risk protection arrangements");

        TaskHelper.updateConfirmAcademyRiskProtectionArrangements(
            taskId,
            ProjectType.Conversion,
            undefined,
            "Commercial",
        );
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the academy's risk protection arrangements");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/risk_protection_arrangement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
