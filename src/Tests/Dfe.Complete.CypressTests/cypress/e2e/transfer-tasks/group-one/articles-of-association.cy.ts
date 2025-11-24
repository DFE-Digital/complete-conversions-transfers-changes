import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";

const taskPath = "articles_of_association";

describe("Transfer tasks - Articles of association", () => {
    let setup: ReturnType<typeof TransferTasksGroupOneSetup.getSetup>;

    before(() => {
        TransferTasksGroupOneSetup.setupProjects();
        setup = TransferTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Partially updating articles of association")
            .hasDropdownContent(
                "Trusts do not have to adopt the latest version of the articles entirely. Although this is DfE's preferred option.",
            )
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to an academy or trust");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage.hasTaskStatusNotApplicable("Articles of association").selectTask("Articles of association");

        Logger.log("Unselect the 'Not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Articles of association").selectTask("Articles of association");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateArticleOfAssociation(setup.taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Articles of association");

        TaskHelperTransfers.updateArticleOfAssociation(setup.taskId, ProjectType.Transfer, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Articles of association");

        TaskHelperTransfers.updateArticleOfAssociation(setup.taskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Articles of association");

        TaskHelperTransfers.updateArticleOfAssociation(setup.taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Articles of association");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/articles_of_association`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
