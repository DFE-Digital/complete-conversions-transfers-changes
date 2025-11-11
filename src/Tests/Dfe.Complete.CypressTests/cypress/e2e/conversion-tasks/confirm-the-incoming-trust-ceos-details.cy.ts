import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
const projectWithoutCEOContact = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.huddersfield,
});
let projectWithoutCEOContactId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
});
let otherUserProjectId: string;

describe("Conversion tasks - Confirm the incoming trust CEO's details", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(projectWithoutCEOContact.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
        });
        projectApi.createAndUpdateConversionProject(projectWithoutCEOContact).then((createResponse) => {
            projectWithoutCEOContactId = createResponse.value;
        });
        projectApi.createAndUpdateMatConversionProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    // awaiting 238981 to add a contact via API
    it.skip("Should", () => {
        cy.visit(`projects/${projectId}/tasks/confirm_incoming_trust_ceo_contact`);
    });

    it("Should see add contact button if no ceo exists", () => {
        cy.visit(`projects/${projectWithoutCEOContactId}/tasks/confirm_incoming_trust_ceo_contact`);
        taskPage.hasButton("Add a contact");
    });

    // awaiting 238981 to add a contact via API
    it.skip("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/confirm_incoming_trust_ceo_contact`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
