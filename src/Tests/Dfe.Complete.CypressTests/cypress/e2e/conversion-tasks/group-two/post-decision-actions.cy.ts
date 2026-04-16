import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "post_decision_actions";

describe("Conversion tasks - Post decision actions", () => {
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
            .hasCheckboxLabel("Application uploaded and verified")
            .expandGuidance("What to check for")
            .hasGuidance("You should check existing project documents, including:")
            .hasCheckboxLabel("Academy Order uploaded and verified")
            .expandGuidance("What to check for")
            .hasGuidance("application to convert")
            .hasCheckboxLabel("LA proforma uploaded and verified")
            .expandGuidance("What to check for")
            .hasGuidance("academy order");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Application uploaded and verified")
            .tick()
            .hasCheckboxLabel("Academy Order uploaded and verified")
            .tick()
            .hasCheckboxLabel("LA proforma uploaded and verified")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Post decision actions")
            .selectTask("Post decision actions");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Application uploaded and verified")
            .untick()
            .hasCheckboxLabel("Academy Order uploaded and verified")
            .untick()
            .hasCheckboxLabel("LA proforma uploaded and verified")
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Post decision actions")
            .selectTask("Post decision actions");

        Logger.log("Select partial checkboxes and save");
        taskPage
            .hasCheckboxLabel("Application uploaded and verified")
            .tick()
            .hasCheckboxLabel("Academy Order uploaded and verified")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Post decision actions");
    });

    checkAccessibilityAcrossPages([
        () => taskListPage.selectTask("Post decision actions"),
        () => taskPage.tick().saveAndReturn(),
    ]);
});