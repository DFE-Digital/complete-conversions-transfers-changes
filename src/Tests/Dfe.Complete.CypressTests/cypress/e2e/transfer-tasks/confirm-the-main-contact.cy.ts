import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
} from "cypress/support/reusableTests";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import { ContactBuilder } from "cypress/api/contactBuilder";
import contactApi from "cypress/api/contactApi";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.coquet,
});
const schoolName = "The Coquet School";
const projectMainContact = ContactBuilder.createContactRequest({
    organisationName: schoolName,
});
let projectId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
});
let otherUserProjectId: string;
const task = "main_contact";

describe("Transfer Tasks - Confirm the main contact", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectMainContact.projectId = { value: projectId };
            contactApi.createContact(projectMainContact);
        });
        projectApi.createAndUpdateMatTransferProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
            contactApi.createContact(ContactBuilder.createContactRequest({ projectId: { value: projectId } }));
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should be able to choose contact and save the task", () => {
        shouldBeAbleToConfirmContact(projectId, projectMainContact.fullName!, task, "Confirm the main contact");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(otherUserProjectId, task);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
