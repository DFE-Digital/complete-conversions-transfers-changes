import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

import { Logger } from "cypress/common/logger";

const taskPath = "confirm_childrens_centre";

const checkboxOptions = [
    "Children's centre remains managed by the local authority",
    "Academy trust intends to manage the children's centre",
    "Staffing and transfer arrangements reviewed",
    "Land, lease and shared use arrangements agreed",
    "Funding, pension and financial liability risks reviewed",
    "Legal agreements and governance requirements reviewed"
];

describe("Conversion tasks - Confirm children's centre provision", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupProjects();
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("for initial status should have all checkboxes unticked", () => {
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
        checkboxOptions.forEach((label) => {
            taskPage.hasCheckboxLabel(label).isUnticked();
        });
    });
    
    it("should submit the form and persist a single selection", () => {
        const option = "Staffing and transfer arrangements reviewed";
        
        taskPage.hasCheckboxLabel(option).tick().saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm children's centre provision")
            .selectTask("Confirm children's centre provision");

        taskPage.hasCheckboxLabel(option).isTicked();
    });

    it("should submit the form and persist multiple selections", () => {
        checkboxOptions.forEach((option) => {
            taskPage.hasCheckboxLabel(option).tick();
        })
        
        taskPage.saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm children's centre provision")
            .selectTask("Confirm children's centre provision");
        
        checkboxOptions.forEach((option) => {
            taskPage.hasCheckboxLabel(option).isTicked();
        })
    });

    it("should persist not applicable option as priority over other selected options", () => {
        const option = "Academy trust intends to manage the children's centre";
        
        taskPage.hasCheckboxLabel("Not applicable").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Confirm children's centre provision")
            .selectTask("Confirm children's centre provision");
        taskPage.hasCheckboxLabel("Not applicable").isTicked();

        taskPage.hasCheckboxLabel(option).tick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Confirm children's centre provision")
            .selectTask("Confirm children's centre provision");
        taskPage.hasCheckboxLabel("Not applicable").isTicked();
        taskPage.hasCheckboxLabel(option).isUnticked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${setup.otherUserProjectId}/tasks/${taskPath}`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
})