import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "share_information";

describe("Conversion tasks - Share the information about opening", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupProjectsWithoutTaskId();
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Email relevant information to your main contact")
            .clickDropdown("What to tell the school or trust")
            .hasGuidance("Email your main contact at the school or trust.");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Confirm' checkbox and save");
        taskPage.hasCheckboxLabel("Email relevant information to your main contact").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Share the information about opening")
            .selectTask("Share the information about opening");

        Logger.log("Unselect the 'Confirm' checkbox and save");
        taskPage
            .hasCheckboxLabel("Email relevant information to your main contact")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Share the information about opening")
            .selectTask("Share the information about opening");
        taskPage.hasCheckboxLabel("Email relevant information to your main contact").isUnticked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
