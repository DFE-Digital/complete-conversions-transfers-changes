import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "articles_of_association";

describe("Conversion tasks - Articles of association", () => {
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
            .clickDropdown("Partially updating articles of association")
            .hasDropdownContent(
                "Trusts do not have to adopt the latest version of the articles entirely. Although this is DfE's preferred option.",
            )
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to a school or trust");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Received' checkbox and save");
        taskPage.hasCheckboxLabel("Received").tick().saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Articles of association").selectTask("Articles of association");

        Logger.log("Unselect the 'Received' checkbox and save");
        taskPage.hasCheckboxLabel("Received").isTicked().untick().saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Articles of association").selectTask("Articles of association");
        taskPage.hasCheckboxLabel("Received").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateArticleOfAssociation(setup.taskId, ProjectType.Conversion, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Articles of association");

        TaskHelperConversions.updateArticleOfAssociation(setup.taskId, ProjectType.Conversion, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Articles of association");

        TaskHelperConversions.updateArticleOfAssociation(setup.taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Articles of association");

        TaskHelperConversions.updateArticleOfAssociation(setup.taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Articles of association");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
