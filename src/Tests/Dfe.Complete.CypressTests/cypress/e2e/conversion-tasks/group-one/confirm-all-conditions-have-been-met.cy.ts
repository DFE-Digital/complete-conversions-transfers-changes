import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";
import allConditionsMetPage from "cypress/pages/projects/conversionsAllConditionsMetPage";

const taskPath = "conditions_met";

describe("Conversion tasks - Confirm all conditions have been met", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjectsWithoutTaskId();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });


    it("should expand and collapse guidance details", () => {
        allConditionsMetPage
            .clickDropdown("How to check all conditions have been met")
            .hasDropdownContent("legal documents are cleared");
    });

    it("for initial status should have No selected", () => {
        allConditionsMetPage.allConditionsMetSection().hasCheckboxLabel("No").isTicked();
        allConditionsMetPage.allConditionsMetSection().hasCheckboxLabel("Yes").isUnticked();

        allConditionsMetPage.shareInformationAboutOpeningSection().hasCheckboxLabel("No").isTicked();
        allConditionsMetPage.shareInformationAboutOpeningSection().hasCheckboxLabel("Yes").isUnticked();
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Yes' option and save");
        allConditionsMetPage.allConditionsMetSection().hasCheckboxLabel("Yes").tick();
        allConditionsMetPage.shareInformationAboutOpeningSection().hasCheckboxLabel("Yes").tick();
        allConditionsMetPage.saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");

        Logger.log("Select the 'No' option and save");
        allConditionsMetPage.allConditionsMetSection().hasCheckboxLabel("No").isUnticked().tick();
        allConditionsMetPage.shareInformationAboutOpeningSection().hasCheckboxLabel("No").isUnticked().tick();
        allConditionsMetPage.saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");

        Logger.log("Select the 'Yes' on conditions met option and save");

        allConditionsMetPage.allConditionsMetSection().hasCheckboxLabel("Yes").isUnticked().tick();
        allConditionsMetPage.shareInformationAboutOpeningSection().hasCheckboxLabel("No").tick();
        allConditionsMetPage.saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");

        Logger.log("Select the 'Yes' on share information about opening option and save");
        allConditionsMetPage.allConditionsMetSection().hasCheckboxLabel("No").isUnticked().tick();
        allConditionsMetPage.shareInformationAboutOpeningSection().hasCheckboxLabel("Yes").isUnticked().tick();
        allConditionsMetPage.saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");
    });


    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        allConditionsMetPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
