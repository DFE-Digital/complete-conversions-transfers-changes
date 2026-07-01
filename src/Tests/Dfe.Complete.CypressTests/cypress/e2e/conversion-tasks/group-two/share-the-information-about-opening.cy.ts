import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "share_information";

describe("Conversion tasks - Share the information about opening", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjectsWithoutTaskId();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });


 it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Yes' option and save");
        taskPage.hasCheckboxLabel("Yes").tick().saveAndReturn();

        taskListPage
            .hasTaskStatusCompleted("Share information about opening")
            .selectTask("Share information about opening");

        Logger.log("Select the 'No' option and save");
        taskPage.hasCheckboxLabel("No").isUnticked().tick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Share information about opening")
            .selectTask("Share information about opening");
        taskPage.hasCheckboxLabel("Yes").isUnticked();
        taskPage.hasCheckboxLabel("No").isTicked();
    });
    
    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
