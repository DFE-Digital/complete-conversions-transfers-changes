import { ContactBuilder } from "cypress/api/contactBuilder";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";
import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { ContactCategory } from "cypress/api/apiDomain";

const projectCEOContact = ContactBuilder.createContactRequest({
    fullName: "CEO McIncomingTrust",
    role: "CEO",
    organisationName: "Spen Valley High School",
    category: ContactCategory.IncomingTrust,
});

const taskPath = "confirm_incoming_trust_ceo_contact";

describe("Conversion tasks - Confirm the incoming trust CEO's details", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupConfirmContactProjects(projectCEOContact, ContactCategory.IncomingTrust);
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
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
