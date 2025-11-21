import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ProjectType } from "cypress/api/taskApi";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "risk_protection_arrangement";

describe("Conversion tasks - Confirm the academy's risk protection arrangements", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupProjects();
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
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
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateConfirmAcademyRiskProtectionArrangements(setup.taskId, ProjectType.Conversion);
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm the academy's risk protection arrangements");

        TaskHelperConversions.updateConfirmAcademyRiskProtectionArrangements(
            setup.taskId,
            ProjectType.Conversion,
            undefined,
            "Standard",
        );
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the academy's risk protection arrangements");

        TaskHelperConversions.updateConfirmAcademyRiskProtectionArrangements(
            setup.taskId,
            ProjectType.Conversion,
            undefined,
            "ChurchOrTrust",
        );
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the academy's risk protection arrangements");

        TaskHelperConversions.updateConfirmAcademyRiskProtectionArrangements(
            setup.taskId,
            ProjectType.Conversion,
            undefined,
            "Commercial",
        );
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the academy's risk protection arrangements");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
