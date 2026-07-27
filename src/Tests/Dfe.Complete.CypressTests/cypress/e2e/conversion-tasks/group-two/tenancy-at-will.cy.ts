import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "tenancy_at_will";

const emailLabel = "Email the school to ask if all relevant parties have agreed and signed the tenancy at will";
const receiveLabel =
    "Receive email from the school confirming all relevant parties have agreed and signed the tenancy at will";
const saveLabel = "Save a copy of the confirmation email in the school's SharePoint folder";

describe("Conversion tasks - Tenancy at will", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjects();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Answer 'Yes', tick all checkboxes and save");
        cy.contains("label", "Yes").first().click();
        taskPage
            .hasCheckboxLabel("Received")
            .tick()
            .hasCheckboxLabel("Cleared")
            .tick()
            .hasCheckboxLabel(emailLabel)
            .tick()
            .hasCheckboxLabel(receiveLabel)
            .tick()
            .hasCheckboxLabel(saveLabel)
            .tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusCompleted("Tenancy at will").selectTask("Tenancy at will");

        Logger.log("Confirm selections persist");
        cy.contains("label", "Yes").first().siblings("input").should("be.checked");
        taskPage
            .hasCheckboxLabel("Received")
            .isTicked()
            .hasCheckboxLabel("Cleared")
            .isTicked()
            .hasCheckboxLabel(emailLabel)
            .isTicked()
            .hasCheckboxLabel(receiveLabel)
            .isTicked()
            .hasCheckboxLabel(saveLabel)
            .isTicked();

        Logger.log("Untick checkboxes and save (radio stays 'Yes' => In progress)");
        taskPage
            .hasCheckboxLabel("Received")
            .untick()
            .hasCheckboxLabel("Cleared")
            .untick()
            .hasCheckboxLabel(emailLabel)
            .untick()
            .hasCheckboxLabel(receiveLabel)
            .untick()
            .hasCheckboxLabel(saveLabel)
            .untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Tenancy at will").selectTask("Tenancy at will");
        taskPage
            .hasCheckboxLabel("Received")
            .isUnticked()
            .hasCheckboxLabel("Cleared")
            .isUnticked()
            .hasCheckboxLabel(emailLabel)
            .isUnticked()
            .hasCheckboxLabel(receiveLabel)
            .isUnticked()
            .hasCheckboxLabel(saveLabel)
            .isUnticked();
    });

    it("should not show the 'Not applicable' checkbox", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("should mark the task complete when both questions are answered 'No'", () => {
        Logger.log("Answer 'No' to both radio questions and save");
        // cy.contains() yields only the first match, so scope each click to its own
        // radio group by the question legend rather than indexing a global 'No' match.
        cy.contains("legend", "Is a tenancy at will being used?")
            .closest("fieldset")
            .within(() => cy.contains("label", "No").click());
        cy.contains("legend", "Is a licence to occupy being used?")
            .closest("fieldset")
            .within(() => cy.contains("label", "No").click());
        taskPage.saveAndReturn();
        taskListPage.hasTaskStatusCompleted("Tenancy at will").selectTask("Tenancy at will");

        Logger.log("Confirm both 'No' answers persist");
        cy.contains("legend", "Is a tenancy at will being used?")
            .closest("fieldset")
            .within(() => cy.contains("label", "No").siblings("input").should("be.checked"));
        cy.contains("legend", "Is a licence to occupy being used?")
            .closest("fieldset")
            .within(() => cy.contains("label", "No").siblings("input").should("be.checked"));
    });

    it("should show task status based on the fields that are selected", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateTenancyAtWill(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Tenancy at will");

        TaskHelperConversions.updateTenancyAtWill(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Tenancy at will");

        TaskHelperConversions.updateTenancyAtWill(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Tenancy at will");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});