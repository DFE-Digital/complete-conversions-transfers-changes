import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { ContactCategory } from "cypress/api/apiDomain";
import { TransferTasksTestSetup } from "cypress/support/transferTasksSetup";

const taskPath = "confirm_outgoing_trust_ceo_contact";
const schoolName = "Coquet Park First School";
const projectCEOContact = ContactBuilder.createContactRequest({
    fullName: "CEO McOutgoingTrust",
    role: "CEO",
    organisationName: schoolName,
    category: ContactCategory.OutgoingTrust,
});

describe("Transfer Tasks - Confirm the outgoing trust CEO's details", () => {
    let setup: ReturnType<typeof TransferTasksTestSetup.getSetup>;

    before(() => {
        TransferTasksTestSetup.setupConfirmContactProjects(projectCEOContact, ContactCategory.OutgoingTrust);
        setup = TransferTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should be able to choose the outgoing trust ceo contact", () => {
        shouldBeAbleToConfirmContact(
            setup.projectId,
            projectCEOContact.fullName!,
            taskPath,
            "Confirm the outgoing trust CEO's details",
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
