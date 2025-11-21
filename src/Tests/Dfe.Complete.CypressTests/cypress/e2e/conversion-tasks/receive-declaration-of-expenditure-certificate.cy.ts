import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ProjectType } from "cypress/api/taskApi";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "receive_grant_payment_certificate";

describe("Conversion tasks - Receive declaration of expenditure certificate", () => {
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
            .clickDropdown("Update the grant assurance spreadsheet")
            .hasDropdownContent("Regional Casework Services team members must update the support grant assurance")
            .hasCheckboxLabel("Check the declaration of expenditure certificate is correct")
            .expandGuidance("Using the right certificate")
            .hasGuidance("If the school only received the £25,000 pre-opening support grant,");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the Not applicable checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");

        Logger.log("Unselect the Not applicable checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        taskListPage.hasTaskStatusNotStarted("Receive declaration of expenditure certificate");

        TaskHelperConversions.updateReceiveDeclarationOfExpenditureCertificate(setup.taskId, ProjectType.Conversion, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Receive declaration of expenditure certificate");

        TaskHelperConversions.updateReceiveDeclarationOfExpenditureCertificate(setup.taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Receive declaration of expenditure certificate");

        TaskHelperConversions.updateReceiveDeclarationOfExpenditureCertificate(setup.taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Receive declaration of expenditure certificate");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
