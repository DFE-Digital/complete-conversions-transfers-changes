import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "church_supplemental_agreement";

describe("Conversion tasks - Church supplemental agreement", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to a school or trust");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .tick()
            .hasCheckboxLabel("Signed by diocese")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Church supplemental agreement")
            .selectTask("Church supplemental agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Signed by diocese")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Church supplemental agreement")
            .selectTask("Church supplemental agreement");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isUnticked()
            .hasCheckboxLabel("Signed by diocese")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Church supplemental agreement");

        TaskHelperConversions.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Church supplemental agreement");

        TaskHelperConversions.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Church supplemental agreement");

        TaskHelperConversions.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Church supplemental agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
