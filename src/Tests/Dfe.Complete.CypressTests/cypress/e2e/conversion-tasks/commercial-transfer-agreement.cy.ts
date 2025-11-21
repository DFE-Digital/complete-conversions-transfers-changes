import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "commercial_transfer_agreement";

describe("Conversion tasks - Commercial transfer agreement", () => {
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
            .clickDropdown("How to check and assure the commercial transfer agreement")
            .hasDropdownContent(
                "You can read guidance about how use check and assure the agreement (opens in new tab) on SharePoint.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Check solicitor responses to assurance questions")
            .tick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Check solicitor responses to assurance questions")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Commercial transfer agreement")
            .selectTask("Commercial transfer agreement");
        taskPage
            .hasCheckboxLabel("Check solicitor responses to assurance questions")
            .isUnticked()
            .hasCheckboxLabel("Save a copy of the confirmation email in the school's SharePoint folder")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateCommercialTransferAgreement(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Commercial transfer agreement");

        TaskHelperConversions.updateCommercialTransferAgreement(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Commercial transfer agreement");

        TaskHelperConversions.updateCommercialTransferAgreement(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Commercial transfer agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
