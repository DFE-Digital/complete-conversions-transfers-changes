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

describe("Conversion tasks - Share the information about opening", () => {
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
        cy.visit(`projects/${projectId}/tasks/share_information`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Email relevant information to your main contact")
            .clickDropdown("What to tell the school or trust")
            .hasGuidance("Email your main contact at the school or trust.");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 'Confirm' checkbox and save");
        taskPage.hasCheckboxLabel("Email relevant information to your main contact").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Share the information about opening")
            .selectTask("Share the information about opening");

        Logger.log("Unselect the 'Confirm' checkbox and save");
        taskPage
            .hasCheckboxLabel("Email relevant information to your main contact")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Share the information about opening")
            .selectTask("Share the information about opening");
        taskPage.hasCheckboxLabel("Email relevant information to your main contact").isUnticked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/share_information`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
