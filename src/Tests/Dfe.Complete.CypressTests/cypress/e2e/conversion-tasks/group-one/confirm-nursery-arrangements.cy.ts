import { Logger } from "cypress/common/logger";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const taskPath = "confirm_nursery_arrangement";

describe("Conversion tasks - Confirm nursery arrangements", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("for initial status should have all checkboxes unticked", () => {
        const checkboxLabels = [
            "Not applicable",
            "A direct provision",
            "Subsidiary Company of the Trust",
            "An independent provider",
            "A children's centre",
        ];
        checkboxLabels.forEach((label) => {
            taskPage.hasCheckboxLabel(label).isUnticked();
        });
    });

    it("for not applicable it should submit the form and persist selection", () => {
        Logger.log(`Select Not applicable`);
        taskPage.hasCheckboxLabel("Not applicable").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Confirm academy nursery arrangement")
            .selectTask("Confirm academy nursery arrangement");

        Logger.log(`Confirm the input for Not applicable has persisted`);
        taskPage.hasCheckboxLabel("Not applicable").isTicked();
    });

    it("should submit the form and persist selection", () => {
        const checkboxLabels = [
            "A direct provision",
            "Subsidiary Company of the Trust",
            "An independent provider",
            "A children's centre",
        ];

        checkboxLabels.forEach((label) => {
            Logger.log(`Select ${label}`);
            taskPage.hasCheckboxLabel(label).tick().saveAndReturn();
            taskListPage
                .hasTaskStatusCompleted("Confirm academy nursery arrangement")
                .selectTask("Confirm academy nursery arrangement");

            Logger.log(`Confirm the input for ${label} has persisted`);
            taskPage.hasCheckboxLabel(label).isTicked();
        });
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
