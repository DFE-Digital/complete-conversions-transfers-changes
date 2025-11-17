import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import contactApi from "cypress/api/contactApi";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
const schoolName = "Spen Valley High School";
const chairOfGovernorsContact = ContactBuilder.createContactRequest({
    fullName: "Chair McGovernor",
    role: "Chair of Governors",
    organisationName: schoolName,
});

const projectWithoutContact = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.huddersfield,
});
let projectWithoutContactId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
});
let otherUserProjectId: string;
const task = "confirm_chair_of_governors_contact";

describe("Conversion tasks - Confirm the chair of governors' details", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(projectWithoutContact.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
            chairOfGovernorsContact.projectId = { value: projectId };
            contactApi.createContact(chairOfGovernorsContact);
        });
        projectApi.createAndUpdateConversionProject(projectWithoutContact).then((createResponse) => {
            projectWithoutContactId = createResponse.value;
        });
        projectApi.createAndUpdateMatConversionProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
            contactApi.createContact(ContactBuilder.createContactRequest({ projectId: { value: otherUserProjectId } }));
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should be able to choose the chair of governors contact", () => {
        shouldBeAbleToConfirmContact(
            projectId,
            chairOfGovernorsContact.fullName!,
            task,
            "Confirm the chair of governors' details",
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
