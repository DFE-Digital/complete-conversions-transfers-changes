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
import contactApi from "cypress/api/contactApi";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.coquet,
});
let projectId: string;
const schoolName = "Coquet Park First School";
const projectHeadteacherContact = ContactBuilder.createContactRequest({
    fullName: "Head McTeacher",
    role: "Headteacher",
    organisationName: schoolName,
});
const projectWithoutContact = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.whitley,
});
let projectWithoutContactId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.marden,
});
let otherUserProjectId: string;
const task = "confirm_headteacher_contact";

describe("Transfer Tasks - Confirm the headteacher's details", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(projectWithoutContact.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectHeadteacherContact.projectId = { value: projectId };
            contactApi.createContact(projectHeadteacherContact);
        });
        projectApi.createAndUpdateTransferProject(projectWithoutContact).then((createResponse) => {
            projectWithoutContactId = createResponse.value;
        });
        projectApi.createAndUpdateMatTransferProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
            contactApi.createContact(ContactBuilder.createContactRequest({ projectId: { value: otherUserProjectId } }));
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should be able to choose the headteacher", () => {
        shouldBeAbleToConfirmContact(
            projectId,
            projectHeadteacherContact.fullName!,
            task,
            "Confirm the headteacher's details",
        );
    });

    it("Should see add contact button if no headteacher or chair of governors contact exists", () => {
        shouldSeeAddContactButtonIfNoContactExists(projectWithoutContactId, task);
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(otherUserProjectId, task);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
