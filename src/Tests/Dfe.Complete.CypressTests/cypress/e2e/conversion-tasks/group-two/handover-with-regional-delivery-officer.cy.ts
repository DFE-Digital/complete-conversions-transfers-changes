import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ProjectType } from "cypress/api/taskApi";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "handover";

describe("Conversion tasks - Handover with regional delivery officer", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjects();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Review the project information, check the documents and carry out research")
            .expandGuidance("What to check for")
            .hasGuidance("You should check existing project documents, including:")
            .hasCheckboxLabel("Make notes and write questions to ask the regional delivery officer")
            .expandGuidance("What to make notes about")
            .hasGuidance("Note down things you want to ask");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the Not applicable checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Handover with regional delivery officer")
            .selectTask("Handover with regional delivery officer");

        Logger.log("Unselect the Not applicable checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Handover with regional delivery officer")
            .selectTask("Handover with regional delivery officer");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateHandoverWithDeliveryOfficer(setup.taskId, ProjectType.Conversion, "notStarted");
        taskListPage.hasTaskStatusNotStarted("Handover with regional delivery officer");

        TaskHelperConversions.updateHandoverWithDeliveryOfficer(setup.taskId, ProjectType.Conversion, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Handover with regional delivery officer");

        TaskHelperConversions.updateHandoverWithDeliveryOfficer(setup.taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Handover with regional delivery officer");

        TaskHelperConversions.updateHandoverWithDeliveryOfficer(setup.taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Handover with regional delivery officer");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
