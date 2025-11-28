import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import validationComponent from "cypress/pages/validationComponent";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "academy_details";

describe("Conversion tasks - Confirm the academy name", () => {
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
            .clickDropdown("Changing the academy name later")
            .hasDropdownContent("Enter the academy name, even if it is the same as the school's name.");
    });

    it("should be able to confirm the academy name", () => {
        Logger.log("Enter academy name and save");
        taskPage.hasCheckboxLabel("Enter the academy name").input("New academy name").saveAndReturn();
        validationComponent.hasNoValidationErrors();
        taskListPage.hasTaskStatusCompleted("Confirm the academy name").selectTask("Confirm the academy name");

        Logger.log("Confirm the input has persisted");
        taskPage.hasCheckboxLabel("Enter the academy name").hasValue("New academy name");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
