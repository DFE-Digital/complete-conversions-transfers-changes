import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "deed_of_variation";

describe("Conversion tasks - Deed of variation", () => {
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
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to a school or trust");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .tick()
            .hasCheckboxLabel("Saved in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Deed of variation").selectTask("Deed of variation");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Deed of variation").selectTask("Deed of variation");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isUnticked()
            .hasCheckboxLabel("Saved in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateDeedOfVariation(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Deed of variation");

        TaskHelperConversions.updateDeedOfVariation(setup.taskId, setup.projectType, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Deed of variation");

        TaskHelperConversions.updateDeedOfVariation(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Deed of variation");

        TaskHelperConversions.updateDeedOfVariation(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Deed of variation");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
