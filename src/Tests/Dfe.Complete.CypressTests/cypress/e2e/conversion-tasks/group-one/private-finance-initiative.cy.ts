import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import taskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "private_finance_initiative";

describe("Conversion tasks - Private finance initiative", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select task options and save");
        cy.contains("label", "Yes").first().click();
        taskPage
            .hasCheckboxLabel("Received")
            .tick()
            .hasCheckboxLabel("Documents sent to SOPU for PFI clearance")
            .tick()
            .hasCheckboxLabel("Cleared")
            .tick()
            .hasCheckboxLabel("Draft saved in trust's SharePoint folder")
            .tick()
            .hasCheckboxLabel("Signed by all stakeholders")
            .tick()
            .hasCheckboxLabel("Final version saved in school and trust's SharePoint folder")
            .tick()
            .saveAndReturn();

        taskListPage.hasTaskStatusInProgress("Private finance initiative").selectTask("Private finance initiative");

        Logger.log("Confirm task selections persist");
        cy.contains("label", "Yes").first().should("have.attr", "for");
        taskPage
            .hasCheckboxLabel("Received")
            .isTicked()
            .hasCheckboxLabel("Documents sent to SOPU for PFI clearance")
            .isTicked()
            .hasCheckboxLabel("Cleared")
            .isTicked()
            .hasCheckboxLabel("Draft saved in trust's SharePoint folder")
            .isTicked()
            .hasCheckboxLabel("Signed by all stakeholders")
            .isTicked()
            .hasCheckboxLabel("Final version saved in school and trust's SharePoint folder")
            .isTicked();
    });

    it("should show task status based on the fields that are selected", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        taskHelperConversions.updatePrivateFinanceInitiative(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Private finance initiative");

        taskHelperConversions.updatePrivateFinanceInitiative(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Private finance initiative");

        taskHelperConversions.updatePrivateFinanceInitiative(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Private finance initiative");

        taskHelperConversions.updatePrivateFinanceInitiative(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Private finance initiative");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
