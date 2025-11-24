import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "supplemental_funding_agreement";

describe("Conversion tasks - Supplemental funding agreement", () => {
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
            .clickDropdown("Help checking the supplemental funding agreement")
            .hasDropdownContent("Changes that personalise the model documents to a school or trust");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Sent to team leader or deputy director")
            .tick()
            .hasCheckboxLabel("Document signed on behalf of the Secretary of State")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Supplemental funding agreement")
            .selectTask("Supplemental funding agreement");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Sent to team leader or deputy director")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Document signed on behalf of the Secretary of State")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Supplemental funding agreement")
            .selectTask("Supplemental funding agreement");
        taskPage
            .hasCheckboxLabel("Sent to team leader or deputy director")
            .isUnticked()
            .hasCheckboxLabel("Document signed on behalf of the Secretary of State")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateSupplementalFundingAgreement(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Supplemental funding agreement");

        TaskHelperConversions.updateSupplementalFundingAgreement(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Supplemental funding agreement");

        TaskHelperConversions.updateSupplementalFundingAgreement(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Supplemental funding agreement");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
