import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "master_funding_agreement";

describe("Conversion tasks - Master funding agreement", () => {
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
            .clickDropdown("Help checking and updating the master funding agreement")
            .hasDropdownContent("Changes that personalise the model documents to a school or trust")
            .clickDropdown("How to sign the master funding agreement")
            .hasDropdownContent("The Secretary of State, or somebody with the authority to act on their behalf");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .tick()
            .hasCheckboxLabel("Saved in school and trust SharePoint folders")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Master funding agreement").selectTask("Master funding agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Saved in school and trust SharePoint folders")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Master funding agreement").selectTask("Master funding agreement");
        taskPage
            .hasCheckboxLabel("Signed by school or trust")
            .isUnticked()
            .hasCheckboxLabel("Saved in school and trust SharePoint folders")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateMasterFundingAgreement(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Master funding agreement");

        TaskHelperConversions.updateMasterFundingAgreement(setup.taskId, setup.projectType, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Master funding agreement");

        TaskHelperConversions.updateMasterFundingAgreement(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Master funding agreement");

        TaskHelperConversions.updateMasterFundingAgreement(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Master funding agreement");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
