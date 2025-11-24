import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { ConversionTasksTestSetup } from "cypress/support/conversionTasksSetup";
import { ContactCategory } from "cypress/api/apiDomain";

const projectMainContact = ContactBuilder.createContactRequest({
    fullName: "Main Contact Person",
    organisationName: "Spen Valley High School",
});

const taskPath = "main_contact";

describe("Conversion tasks - Confirm the main contact", () => {
    let setup: ReturnType<typeof ConversionTasksTestSetup.getSetup>;

    before(() => {
        ConversionTasksTestSetup.setupConfirmContactProjects(projectMainContact, ContactCategory.SchoolOrAcademy);
        setup = ConversionTasksTestSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksTestSetup.setupBeforeEach(taskPath);
    });

    it("Should be able to choose contact and save the task", () => {
        shouldBeAbleToConfirmContact(setup.projectId, projectMainContact.fullName!, taskPath, "Confirm the main contact");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(setup.otherUserProjectId, taskPath);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
