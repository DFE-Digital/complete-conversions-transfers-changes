import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: { value: urnPool.conversionTasks.spen },
});
let projectId: string;
const projectWithoutContact = ProjectBuilder.createConversionProjectRequest({
    urn: { value: urnPool.conversionTasks.huddersfield },
});
let projectWithoutContactId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: urnPool.conversionTasks.grylls },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Conversion Tasks - Confirm the headteacher's details", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectRemover.removeProjectIfItExists(projectWithoutContact.urn.value);
        projectRemover.removeProjectIfItExists(otherUserProject.urn.value);
        projectApi.createConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
        });
        projectApi.createConversionProject(projectWithoutContact).then((createResponse) => {
            projectWithoutContactId = createResponse.value;
        });
        projectApi.createMatConversionProject(otherUserProject).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    // awaiting 238981 to add a contact via API
    it.skip("Should be able to choose the headteacher", () => {
        cy.visit(`projects/${projectId}/tasks/confirm_headteacher_contact`);
    });

    it("Should see add contact button if no headteacher or chair of governors contact exists", () => {
        cy.visit(`projects/${projectWithoutContactId}/tasks/confirm_headteacher_contact`);
        taskPage.hasButton("Add a contact");
    });

    // awaiting 238981 to add a contact via API
    it.skip("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/confirm_headteacher_contact`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
