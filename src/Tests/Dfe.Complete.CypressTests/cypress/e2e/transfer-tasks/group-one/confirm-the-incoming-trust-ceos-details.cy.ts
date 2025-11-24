import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { ContactCategory } from "cypress/api/apiDomain";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";

const taskPath = "confirm_incoming_trust_ceo_contact";
const schoolName = "Coquet High School";
const projectCEOContact = ContactBuilder.createContactRequest({
    fullName: "CEO McIncomingTrust",
    role: "CEO",
    organisationName: schoolName,
    category: ContactCategory.IncomingTrust,
});

describe("Transfer Tasks - Confirm the incoming trust CEO's details", () => {
    let setup: ReturnType<typeof TransferTasksGroupOneSetup.getSetup>;

    before(() => {
        TransferTasksGroupOneSetup.setupConfirmContactProjects(projectCEOContact, ContactCategory.IncomingTrust);
        setup = TransferTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should be able to choose the incoming trust ceo contact", () => {
        shouldBeAbleToConfirmContact(
            setup.projectId,
            projectCEOContact.fullName!,
            taskPath,
            "Confirm the incoming trust CEO's details",
        );
    });

    it("Should see add contact button if no ceo exists", () => {
        shouldSeeAddContactButtonIfNoContactExists(setup.projectWithoutContactId!, taskPath);
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(setup.otherUserProjectId, taskPath);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
