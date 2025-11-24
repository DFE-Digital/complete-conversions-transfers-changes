import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
    shouldSeeAddContactButtonIfNoContactExists,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";
import { ContactCategory } from "cypress/api/apiDomain";

const taskPath = "confirm_headteacher_contact";
const schoolName = "Coquet Park First School";
const projectHeadteacherContact = ContactBuilder.createContactRequest({
    fullName: "Head McTeacher",
    role: "Headteacher",
    organisationName: schoolName,
});

describe("Transfer Tasks - Confirm the headteacher's details", () => {
    let setup: ReturnType<typeof TransferTasksGroupOneSetup.getSetup>;

    before(() => {
        TransferTasksGroupOneSetup.setupConfirmContactProjects(projectHeadteacherContact, ContactCategory.SchoolOrAcademy);
        setup = TransferTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
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
