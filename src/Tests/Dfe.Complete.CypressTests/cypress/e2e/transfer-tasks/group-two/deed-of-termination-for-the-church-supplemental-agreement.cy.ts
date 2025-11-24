import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "deed_termination_church_agreement";

describe("Transfers tasks - Deed of termination for the church supplemental agreement", () => {
    let setup: ReturnType<typeof TransferTasksTestSetup.getSetup>;

    before(() => {
        TransferTasksTestSetup.setupProjects();
        setup = TransferTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        TransferTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Other ways to deal with a church supplemental agreement")
            .hasDropdownContent("Solicitors may want to transfer a church supplemental agreement to the new trust")
            .clickDropdown("Using a deed of termination to transfer a church supplemental agreement")
            .hasDropdownContent("There is no model document for using a deed of termination to transfer")
            .clickDropdown("How to sign the deed of termination")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'not applicable' checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Deed of termination for the church supplemental agreement")
            .selectTask("Deed of termination for the church supplemental agreement");

        Logger.log("Unselect 'not applicable' checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Deed of termination for the church supplemental agreement")
            .selectTask("Deed of termination for the church supplemental agreement");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateDeedOfTerminationChurchSupplementalAgreement(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Deed of termination for the church supplemental agreement");

        TaskHelperTransfers.updateDeedOfTerminationChurchSupplementalAgreement(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Deed of termination for the church supplemental agreement");

        TaskHelperTransfers.updateDeedOfTerminationChurchSupplementalAgreement(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Deed of termination for the church supplemental agreement");

        TaskHelperTransfers.updateDeedOfTerminationChurchSupplementalAgreement(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Deed of termination for the church supplemental agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/deed_termination_church_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
