import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupTwoSetup } from "cypress/support/transferTasksSetup";

const taskPath = "deed_of_novation_and_variation";

describe("Transfers tasks - Deed of novation and variation", () => {
    let setup: ReturnType<typeof TransferTasksGroupTwoSetup.getSetup>;

    before(() => {
        TransferTasksGroupTwoSetup.setupProjects();
        setup = TransferTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to an academy or trust are expected.")
            .clickDropdown("How to sign the deed of novation and variation")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'signed by the secretary of state' checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .tick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Deed of novation and variation")
            .selectTask("Deed of novation and variation");

        Logger.log("Unselect 'signed by the secretary of state' checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Deed of novation and variation")
            .selectTask("Deed of novation and variation");
        taskPage
            .hasCheckboxLabel("Signed on behalf of Secretary of State")
            .isUnticked()
            .hasCheckboxLabel("Saved in the academy SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateDeedOfNovationAndVariation(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Deed of novation and variation");

        TaskHelperTransfers.updateDeedOfNovationAndVariation(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Deed of novation and variation");

        TaskHelperTransfers.updateDeedOfNovationAndVariation(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Deed of novation and variation");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/deed_of_novation_and_variation`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
