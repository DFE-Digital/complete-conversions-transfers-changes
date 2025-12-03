import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

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
        taskPage
            .clickDropdown("How to check all conditions have been met")
            .hasDropdownContent("legal documents are cleared")
            .clickDropdown("What to do if conditions are not met")
            .hasDropdownContent(
                "You must agree a new conversion date with all stakeholders, then change the conversion date for this project.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Confirm' checkbox and save");
        taskPage.hasCheckboxLabel("Confirm all conditions are met").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");

        Logger.log("Unselect the 'Confirm' checkbox and save");
        taskPage.hasCheckboxLabel("Confirm all conditions are met").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");
        taskPage.hasCheckboxLabel("Confirm all conditions are met").isUnticked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
