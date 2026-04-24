import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "tupe_consultation";

describe("Conversion tasks - TUPE Consultation", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should display the TUPE consultation information", () => {
        taskPage
            .contains("TUPE (Transfer of Undertakings [Protection of Employment]) is a legal process")
            .contains("As part of TUPE, all staff and unions must be consulted")
            .contains("DfE requires confirmation that the TUPE consultation has been completed");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm that TUPE consultation and any associated issues are being managed.")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("TUPE Consultation")
            .selectTask("TUPE Consultation");

        Logger.log("Unselect checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm that TUPE consultation and any associated issues are being managed.")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("TUPE Consultation")
            .selectTask("TUPE Consultation");
        taskPage
            .hasCheckboxLabel("Confirm that TUPE consultation and any associated issues are being managed.")
            .isUnticked();
    });

    it("should show task status based on the checkbox that is checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateTupeConsultation(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("TUPE Consultation");

        TaskHelperConversions.updateTupeConsultation(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("TUPE Consultation");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});