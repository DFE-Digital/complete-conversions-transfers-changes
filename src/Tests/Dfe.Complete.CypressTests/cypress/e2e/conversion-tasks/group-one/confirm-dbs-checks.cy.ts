import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "confirm_dbs_checks";

describe("Conversion tasks - Confirm DBS Checks", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });


    it("should submit the form and persist selections", () => {
        Logger.log("Select checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm DBS checks are taking place/being reviewed")
            .tick()
            .saveAndReturn();

        taskListPage
            .hasTaskStatusCompleted("DBS checks")
            .selectTask("DBS checks");

        Logger.log("Unselect checkbox and save");
        taskPage
            .hasCheckboxLabel("Confirm DBS checks are taking place/being reviewed")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("DBS checks");
    });


    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
