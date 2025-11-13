import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import validationComponent from "cypress/pages/validationComponent";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
});
let otherUserProjectId: string;

describe("Conversion tasks - Confirm the academy name", () => {
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
        cy.visit(`projects/${projectId}/tasks/academy_details`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Changing the academy name later")
            .hasDropdownContent("Enter the academy name, even if it is the same as the school's name.");
    });

    it("should be able to confirm the academy name", () => {
        Logger.log("Enter academy name and save");
        taskPage.hasCheckboxLabel("Enter the academy name").input("New academy name").saveAndReturn();
        validationComponent.hasNoValidationErrors();
        taskListPage.hasTaskStatusCompleted("Confirm the academy name").selectTask("Confirm the academy name");

        Logger.log("Confirm the input has persisted");
        taskPage.hasCheckboxLabel("Enter the academy name").hasValue("New academy name");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/academy_details`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
