import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";

const taskPath = "church_supplemental_agreement";

describe("Transfer tasks - Church supplemental agreement", () => {
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
            .clickDropdown("Help checking for changes")
            .hasDropdownContent("Changes that personalise the model documents to an academy or trust")
            .clickDropdown("How to sign the Church Supplemental Agreement")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select 'Not applicable' and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Church supplemental agreement")
            .selectTask("Church supplemental agreement");

        Logger.log("Unselect 'Not applicable' and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Church supplemental agreement")
            .selectTask("Church supplemental agreement");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperTransfers.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Church supplemental agreement");

        TaskHelperTransfers.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Church supplemental agreement");

        TaskHelperTransfers.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Church supplemental agreement");

        TaskHelperTransfers.updateChurchSupplementalAgreement(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Church supplemental agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/church_supplemental_agreement`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
