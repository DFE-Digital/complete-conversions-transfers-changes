import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "land_questionnaire";

describe("Conversion tasks - Land questionnaire", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupProjects();
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to clear a land questionnaire")
            .hasDropdownContent("You must check the school is using the right land questionnaire");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .tick()
            .hasCheckboxLabel("Cleared")
            .tick()
            .hasCheckboxLabel("Signed by solicitor")
            .tick()
            .hasCheckboxLabel("Saved in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusCompleted("Land questionnaire").selectTask("Land questionnaire");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Received")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Cleared")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Signed by solicitor")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Land questionnaire").selectTask("Land questionnaire");
        taskPage
            .hasCheckboxLabel("Received")
            .isUnticked()
            .hasCheckboxLabel("Cleared")
            .isUnticked()
            .hasCheckboxLabel("Signed by solicitor")
            .isUnticked()
            .hasCheckboxLabel("Saved in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateLandQuestionnaire(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Land questionnaire");

        TaskHelperConversions.updateLandQuestionnaire(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Land questionnaire");

        TaskHelperConversions.updateLandQuestionnaire(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Land questionnaire");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
