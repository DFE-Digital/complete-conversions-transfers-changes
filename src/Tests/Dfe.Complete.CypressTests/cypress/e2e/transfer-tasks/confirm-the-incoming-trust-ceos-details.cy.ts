import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { ContactCategory } from "cypress/api/apiDomain";
import contactApi from "cypress/api/contactApi";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.coquet,
});
const schoolName = "Coquet High School";
const projectCEOContact = ContactBuilder.createContactRequest({
    fullName: "CEO McIncomingTrust",
    role: "CEO",
    organisationName: schoolName,
    category: ContactCategory.IncomingTrust,
});
let projectId: string;
const projectWithoutCEOContact = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.whitley,
});
let projectWithoutCEOContactId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
});
let otherUserProjectId: string;
const task = "confirm_incoming_trust_ceo_contact";

describe("Transfer Tasks - Confirm the incoming trust CEO's details", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(projectWithoutCEOContact.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectCEOContact.projectId = { value: projectId };
            contactApi.createContact(projectCEOContact);
        });
        projectApi.createAndUpdateTransferProject(projectWithoutCEOContact).then((createResponse) => {
            projectWithoutCEOContactId = createResponse.value;
        });
        projectApi.createAndUpdateMatTransferProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
            contactApi.createContact(
                ContactBuilder.createContactRequest({
                    projectId: { value: otherUserProjectId },
                    category: ContactCategory.IncomingTrust,
                }),
            );
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should be able to choose the incoming trust ceo contact", () => {
        shouldBeAbleToConfirmContact(
            projectId,
            projectCEOContact.fullName!,
            task,
            "Confirm the incoming trust CEO's details",
        );
    });

    it("Should see add contact button if no ceo exists", () => {
        shouldSeeAddContactButtonIfNoContactExists(projectWithoutCEOContactId, task);
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(otherUserProjectId, task);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
