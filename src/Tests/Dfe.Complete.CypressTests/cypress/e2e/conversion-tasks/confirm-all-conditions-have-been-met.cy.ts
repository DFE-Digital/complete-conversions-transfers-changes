import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
});
let otherUserProjectId: string;

describe("Conversion tasks - Confirm all conditions have been met", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
        });
        projectApi.createAndUpdateMatConversionProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/conditions_met`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("How to check all conditions have been met")
            .hasDropdownContent("legal documents are cleared")
            .clickDropdown("What to do if conditions are not met")
            .hasDropdownContent(
                "You must agree a new conversion date with all stakeholders, then change the conversion date for this project.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Confirm' checkbox and save");
        taskPage.hasCheckboxLabel("Confirm all conditions are met").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");

        Logger.log("Unselect the 'Confirm' checkbox and save");
        taskPage.hasCheckboxLabel("Confirm all conditions are met").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Confirm all conditions have been met")
            .selectTask("Confirm all conditions have been met");
        taskPage.hasCheckboxLabel("Confirm all conditions are met").isUnticked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/conditions_met`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
