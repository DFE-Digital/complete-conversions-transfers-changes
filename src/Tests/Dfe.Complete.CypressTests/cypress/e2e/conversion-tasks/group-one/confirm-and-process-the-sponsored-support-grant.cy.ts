import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";
import taskHelper from "cypress/api/taskHelper";

const taskPath = "sponsored_support_grant";

describe("Conversion tasks - Confirm and process the sponsored support grant", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to check grant eligibility")
            .hasDropdownContent("The fast track grant amounts are")
            .clickDropdown("Changing the grant amount if the trust needs more funding later")
            .hasDropdownContent("Trusts can discover new information")
            .hasCheckboxLabel("Tell the trust how much they are entitled to and how to claim the grant")
            .expandGuidance("How to claim different grants")
            .hasGuidance("The sponsor can claim for the fast track grant first")
            .hasCheckboxLabel("Send the grant information to the payments team")
            .expandGuidance("What information to send")
            .hasGuidance("You'll find the directive academy order and grant claim form")
            .hasCheckboxLabel("Tell the trust the grant is being processed")
            .expandGuidance("What to do if the trust do not receive the grant")
            .hasGuidance("Email the Grant Payments team at");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select some checkboxes and save");
        taskPage
            .hasCheckboxLabel("Tell the trust how much they are entitled to and how to claim the grant")
            .tick()
            .hasCheckboxLabel("Send the grant information to the payments team")
            .tick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Confirm and process the sponsored support grant")
            .selectTask("Confirm and process the sponsored support grant");

        Logger.log("Unselect same checkboxes and save");
        taskPage
            .hasCheckboxLabel("Tell the trust how much they are entitled to and how to claim the grant")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Send the grant information to the payments team")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm and process the sponsored support grant")
            .selectTask("Confirm and process the sponsored support grant");
        taskPage
            .hasCheckboxLabel("Tell the trust how much they are entitled to and how to claim the grant")
            .isUnticked()
            .hasCheckboxLabel("Send the grant information to the payments team")
            .isUnticked();
    });

    it("should show task status based on the checkboxes that are checked", () => {
        cy.visit(`projects/${setup.projectId}/tasks`);

        taskHelper.updateSponsoredSupportGrant(setup.taskId, setup.projectType, "notStarted");
        cy.reload();
        taskListPage.hasTaskStatusNotStarted("Confirm and process the sponsored support grant");

        taskHelper.updateSponsoredSupportGrant(setup.taskId, setup.projectType, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Confirm and process the sponsored support grant");

        taskHelper.updateSponsoredSupportGrant(setup.taskId, setup.projectType, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Confirm and process the sponsored support grant");

        taskHelper.updateSponsoredSupportGrant(setup.taskId, setup.projectType, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Confirm and process the sponsored support grant");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
