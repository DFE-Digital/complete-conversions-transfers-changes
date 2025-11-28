import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupTwoSetup } from "cypress/support/transferTasksSetup";

const taskPath = "sponsored_support_grant";

describe("Transfers tasks - Confirm transfer grant funding level\n", () => {
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
            .clickDropdown("Grant amounts, eligibility and guidance")
            .hasDropdownContent("Each eligible transfer will get £25,000 as a standard level of funding.")
            .clickDropdown("Changing the grant amount if the trust needs more funding later")
            .hasDropdownContent("Trusts can discover new information");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Confirm transfer grant funding level")
            .selectTask("Confirm transfer grant funding level");

        Logger.log("Unselect 'not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm transfer grant funding level")
            .selectTask("Confirm transfer grant funding level");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateSponsoredSupportGrant(setup.taskId, ProjectType.Transfer, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm transfer grant funding level");

        TaskHelperTransfers.updateSponsoredSupportGrant(setup.taskId, ProjectType.Transfer, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Confirm transfer grant funding level");

        TaskHelperTransfers.updateSponsoredSupportGrant(setup.taskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm transfer grant funding level");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
