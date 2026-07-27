import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "third_party_leases";
const emailLabel = "Email the solicitors to ask if all relevant parties have agreed and signed the lease";
const receiveLabel = "Receive email from the solicitors confirming all relevant parties have agreed and signed the lease";
const saveLabel = "Save a copy of the confirmation email in the school's SharePoint folder";

describe("Conversion tasks - Third party leases", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjects();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select checkboxes and save");
        taskPage
            .hasCheckboxLabel(emailLabel).tick()
            .hasCheckboxLabel(receiveLabel).tick()
            .saveAndReturn();
        taskListPage.hasTaskStatusInProgress("Third party leases").selectTask("Third party leases");

        Logger.log("Unselect checkboxes and save");
        taskPage
            .hasCheckboxLabel(emailLabel).isTicked().untick()
            .hasCheckboxLabel(receiveLabel).isTicked().untick()
            .saveAndReturn();
        taskListPage.hasTaskStatusNotStarted("Third party leases").selectTask("Third party leases");
        taskPage
            .hasCheckboxLabel(emailLabel).isUnticked()
            .hasCheckboxLabel(receiveLabel).isUnticked()
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateThirdPartyLeases(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Third party leases");

        TaskHelperConversions.updateThirdPartyLeases(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Third party leases");

        TaskHelperConversions.updateThirdPartyLeases(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Third party leases");

        TaskHelperConversions.updateThirdPartyLeases(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Third party leases");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});