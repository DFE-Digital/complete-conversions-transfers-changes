import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "master_funding_agreement";

describe("Conversion tasks - Master funding agreement", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjects();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should contain guidance details", () => {
        taskPage
            .hasDropdownContent(
                "The Master Funding Agreement (MFA) is a deed between the DfE and academy trust and outlines the core teams and conditions under which the trust operates",
            )
            .hasDropdownContent(
                "Changes that personalise the model documents to a school or trust, and remove or add optional clauses, are expected",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by trust")
            .tick()
            .hasCheckboxLabel("Signed and sealed on behalf of SoS")
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Master funding agreement").selectTask("Master funding agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Signed by trust")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Signed and sealed on behalf of SoS")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Master funding agreement").selectTask("Master funding agreement");
        taskPage
            .hasCheckboxLabel("Signed by trust")
            .isUnticked()
            .hasCheckboxLabel("Signed and sealed on behalf of SoS")
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
