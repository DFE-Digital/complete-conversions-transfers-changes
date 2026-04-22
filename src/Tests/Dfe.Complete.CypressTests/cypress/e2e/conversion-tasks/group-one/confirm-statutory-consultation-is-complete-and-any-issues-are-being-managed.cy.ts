import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "confirm_statutory_consultation";

describe("Conversion tasks - Confirm statutory consultation is complete and any issues are being managed", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("Should submit the form and persist selections", () => {
        Logger.log("Select the 'Consultation(s) complete and any issues being managed' option and save");
        taskPage
            .hasCheckboxLabel("Consultation(s) complete and any issues being managed")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm statutory consultation is complete and any issues are being managed")
            .selectTask("Confirm statutory consultation is complete and any issues are being managed");

        Logger.log("Change to 'Not applicable' selection");
        taskPage
            .hasCheckboxLabel("Consultation(s) complete and any issues being managed")
            .isTicked()
            .hasCheckboxLabel("Not applicable")
            .isUnticked()
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Confirm statutory consultation is complete and any issues are being managed")
            .selectTask("Confirm statutory consultation is complete and any issues are being managed");
        
        taskPage.hasCheckboxLabel("Not applicable").isTicked();
        taskPage.hasCheckboxLabel("Consultation(s) complete and any issues being managed").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateConfirmStatutoryConsultation(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm statutory consultation is complete and any issues are being managed");

        TaskHelperConversions.updateConfirmStatutoryConsultation(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Confirm statutory consultation is complete and any issues are being managed");

        TaskHelperConversions.updateConfirmStatutoryConsultation(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm statutory consultation is complete and any issues are being managed");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});