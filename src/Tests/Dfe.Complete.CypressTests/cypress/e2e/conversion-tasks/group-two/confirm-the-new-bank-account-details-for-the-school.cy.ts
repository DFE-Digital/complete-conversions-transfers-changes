import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksGroupTwoSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "confirm_school_bank_details";

describe("Conversion tasks - Confirm the new bank account details for the school", () => {
    let setup: ReturnType<typeof ConversionTasksGroupTwoSetup.getSetup>;

    before(() => {
        ConversionTasksGroupTwoSetup.setupProjects();
        setup = ConversionTasksGroupTwoSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupTwoSetup.setupBeforeEach(taskPath);
    });

    it("should render guidance and checkbox options", () => {
        taskPage
            .hasCheckboxLabel(
                "Request confirmation that the school's bank account details have been sent to the DfE using the automated form by the published deadline.",
            )
            .hasCheckboxLabel("Received confirmation that the school bank details have been submitted.");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select status checkboxes and save");
        taskPage
            .hasCheckboxLabel(
                "Request confirmation that the school's bank account details have been sent to the DfE using the automated form by the published deadline.",
            )
            .tick()
            .hasCheckboxLabel("Received confirmation that the school bank details have been submitted.")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm the new bank account details for the school")
            .selectTask("Confirm the new bank account details for the school");

        Logger.log("Unset status checkboxes and save");
        taskPage
            .hasCheckboxLabel(
                "Request confirmation that the school's bank account details have been sent to the DfE using the automated form by the published deadline.",
            )
            .isTicked()
            .untick()
            .hasCheckboxLabel("Received confirmation that the school bank details have been submitted.")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm the new bank account details for the school")
            .selectTask("Confirm the new bank account details for the school");
        taskPage
            .hasCheckboxLabel(
                "Request confirmation that the school's bank account details have been sent to the DfE using the automated form by the published deadline.",
            )
            .isUnticked()
            .hasCheckboxLabel("Received confirmation that the school bank details have been submitted.")
            .isUnticked();
    });


    it("should show task status based on selected options", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateConfirmSchoolBankDetails(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm the new bank account details for the school");

        TaskHelperConversions.updateConfirmSchoolBankDetails(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Confirm the new bank account details for the school");

        TaskHelperConversions.updateConfirmSchoolBankDetails(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm the new bank account details for the school");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
