import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { ConversionTasksGroupOneSetup } from "cypress/support/conversionTasksSetup";
import { ContactCategory } from "cypress/api/apiDomain";

const projectHeadteacherContact = ContactBuilder.createContactRequest({
    fullName: "Head McTeacher",
    role: "Headteacher",
    organisationName: "Spen Valley High School",
});

const taskPath = "confirm_headteacher_contact";

describe("Conversion tasks - Confirm the headteacher's details", () => {
    let setup: ReturnType<typeof ConversionTasksGroupOneSetup.getSetup>;

    before(() => {
        ConversionTasksGroupOneSetup.setupConfirmContactProjects(
            projectHeadteacherContact,
            ContactCategory.SchoolOrAcademy,
        );
        setup = ConversionTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        ConversionTasksGroupOneSetup.setupBeforeEach(taskPath);
    });

    it("Should be able to choose the headteacher", () => {
        shouldBeAbleToConfirmContact(
            setup.projectId,
            projectHeadteacherContact.fullName!,
            taskPath,
            "Confirm the headteacher's details",
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
