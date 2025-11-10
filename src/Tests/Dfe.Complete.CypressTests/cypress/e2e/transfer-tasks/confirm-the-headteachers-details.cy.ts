import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { rdoLondonUser } from "cypress/constants/cypressConstants";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.coquet,
});
let projectId: string;
const projectWithoutContact = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.whitley,
});
let projectWithoutContactId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
});
let otherUserProjectId: string;

describe("Transfer Tasks - Confirm the headteacher's details", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(projectWithoutContact.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
        });
        projectApi.createAndUpdateTransferProject(projectWithoutContact).then((createResponse) => {
            projectWithoutContactId = createResponse.value;
        });
        projectApi.createAndUpdateMatTransferProject(otherUserProject, rdoLondonUser).then((createResponse) => {
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
