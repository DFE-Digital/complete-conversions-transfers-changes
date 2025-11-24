import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";

const taskPath = "conversion_grant";

describe("Conversion tasks - Process conversion support grant", () => {
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
            .clickDropdown("How to process a conversion support grant")
            .hasDropdownContent("Check the grant claim form is ready")
            .hasCheckboxLabel("Check the school or trust have a vendor account")
            .expandGuidance("How to set up a vendor account")
            .hasGuidance("The school or trust receiving the conversion support grant must have a vendor account.")
            .hasCheckboxLabel("Receive, check and save the grant claim form in the school's SharePoint folder")
            .expandGuidance("How to check and save the form")
            .hasGuidance("The school or trust must send you the grant claim form.");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Send the grant information and vendor account details to the payments team")
            .tick()
            .hasCheckboxLabel("Tell the school or trust the grant information has been sent")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Process conversion support grant")
            .selectTask("Process conversion support grant");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Send the grant information and vendor account details to the payments team")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Tell the school or trust the grant information has been sent")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Process conversion support grant")
            .selectTask("Process conversion support grant");
        taskPage
            .hasCheckboxLabel("Send the grant information and vendor account details to the payments team")
            .isUnticked()
            .hasCheckboxLabel("Tell the school or trust the grant information has been sent")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        TaskHelperConversions.updateProcessConversionSupportGrant(setup.taskId, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Process conversion support grant");

        TaskHelperConversions.updateProcessConversionSupportGrant(setup.taskId, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Process conversion support grant");

        TaskHelperConversions.updateProcessConversionSupportGrant(setup.taskId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Process conversion support grant");

        TaskHelperConversions.updateProcessConversionSupportGrant(setup.taskId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Process conversion support grant");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
