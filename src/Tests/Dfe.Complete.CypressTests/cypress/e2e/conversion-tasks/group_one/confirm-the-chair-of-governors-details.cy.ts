import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";
import { ContactCategory } from "cypress/api/apiDomain";

const chairOfGovernorsContact = ContactBuilder.createContactRequest({
    fullName: "Chair McGovernor",
    role: "Chair of Governors",
    organisationName: "Spen Valley High School",
});

const taskPath = "confirm_chair_of_governors_contact";

describe("Conversion tasks - Confirm the chair of governors' details", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupConfirmContactProjects(chairOfGovernorsContact, ContactCategory.SchoolOrAcademy);
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("Should be able to choose the chair of governors contact", () => {
        shouldBeAbleToConfirmContact(
            setup.projectId,
            chairOfGovernorsContact.fullName!,
            taskPath,
            "Confirm the chair of governors' details",
        );
    });

    it("Should see add contact button if no headteacher or chair of governors contact exists", () => {
        shouldSeeAddContactButtonIfNoContactExists(setup.projectWithoutContactId!, taskPath);
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(setup.otherUserProjectId, taskPath);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
