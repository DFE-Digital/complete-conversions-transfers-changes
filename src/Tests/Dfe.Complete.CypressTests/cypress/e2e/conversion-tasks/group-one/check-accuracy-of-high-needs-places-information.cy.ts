import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "check_accuracy_of_higher_needs";

describe("Conversion tasks - Check accuracy of high needs places", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Confirm the section 251 spreadsheet shows the correct number of high needs places FFO must fund")
            .tick()
            .hasCheckboxLabel("Tell the local authority to complete the Notification of Changes form")
            .tick()
            .hasCheckboxLabel("Check the returned Notification of Changes form")
            .tick()
            .hasCheckboxLabel("Send the completed Notification of Changes form to FFO")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Check accuracy of high needs places")
            .selectTask("Check accuracy of high needs places");

        Logger.log("Unselect all checkboxes and save");
        taskPage
            .hasCheckboxLabel("Confirm the section 251 spreadsheet shows the correct number of high needs places FFO must fund")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Tell the local authority to complete the Notification of Changes form")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Check the returned Notification of Changes form")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Send the completed Notification of Changes form to FFO")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Check accuracy of high needs places")
            .selectTask("Check accuracy of high needs places");
        taskPage
            .hasCheckboxLabel("Confirm the section 251 spreadsheet shows the correct number of high needs places FFO must fund")
            .isUnticked()
            .hasCheckboxLabel("Tell the local authority to complete the Notification of Changes form")
            .isUnticked()
            .hasCheckboxLabel("Check the returned Notification of Changes form")
            .isUnticked()
            .hasCheckboxLabel("Send the completed Notification of Changes form to FFO")
            .isUnticked();
    });

    it("should have EXCLUSIVE not applicable checkbox", () => {
        Logger.log("Check all checkboxes are unticked");
        taskPage
            .hasCheckboxLabel("Not applicable")
            .isUnticked()
            .hasCheckboxLabel("Confirm the section 251 spreadsheet shows the correct number of high needs places FFO must fund")
            .isUnticked()
            .hasCheckboxLabel("Tell the local authority to complete the Notification of Changes form")
            .isUnticked()
            .hasCheckboxLabel("Check the returned Notification of Changes form")
            .isUnticked()
            .hasCheckboxLabel("Send the completed Notification of Changes form to FFO")
            .isUnticked();

        Logger.log("Check ticking other options unticks not applicable");
        taskPage.hasCheckboxLabel("Not applicable").tick();
        taskPage.hasCheckboxLabel("Send the completed Notification of Changes form to FFO").tick();

        taskPage
            .hasCheckboxLabel("Not applicable")
            .isUnticked()
            .hasCheckboxLabel("Send the completed Notification of Changes form to FFO")
            .isTicked();

        Logger.log("Check ticking not applicable unticks other options");
        taskPage
            .hasCheckboxLabel("Confirm the section 251 spreadsheet shows the correct number of high needs places FFO must fund")
            .hasCheckboxLabel("Not applicable").tick();

        taskPage
            .hasCheckboxLabel("Not applicable")
            .isTicked()
            .hasCheckboxLabel("Confirm the section 251 spreadsheet shows the correct number of high needs places FFO must fund")
            .isUnticked()
            .hasCheckboxLabel("Send the completed Notification of Changes form to FFO")
            .isUnticked();
    })

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateCheckAccuracyOfHigherNeeds(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Check accuracy of high needs places");

        TaskHelperConversions.updateCheckAccuracyOfHigherNeeds(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Check accuracy of high needs places");

        TaskHelperConversions.updateCheckAccuracyOfHigherNeeds(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Check accuracy of high needs places");

        TaskHelperConversions.updateCheckAccuracyOfHigherNeeds(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Check accuracy of high needs places");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
