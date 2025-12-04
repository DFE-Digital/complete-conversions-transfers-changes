import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToConfirmContact,
    shouldNotSeeSaveAndReturnButtonForAnotherUsersProject,
} from "cypress/support/reusableTests";
import { ContactBuilder } from "cypress/api/contactBuilder";
import { TransferTasksGroupOneSetup } from "cypress/support/transferTasksSetup";
import { ContactCategory } from "cypress/api/apiDomain";

const taskPath = "main_contact";
const schoolName = "The Coquet School";
const projectMainContact = ContactBuilder.createContactRequest({
    organisationName: schoolName,
});

describe("Transfer Tasks - Confirm the main contact", () => {
    let setup: ReturnType<typeof TransferTasksGroupOneSetup.getSetup>;

    before(() => {
        TransferTasksGroupOneSetup.setupConfirmContactProjects(projectMainContact, ContactCategory.SchoolOrAcademy);
        setup = TransferTasksGroupOneSetup.getSetup();
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should be able to choose contact and save the task", () => {
        shouldBeAbleToConfirmContact(
            setup.projectId,
            projectMainContact.fullName!,
            taskPath,
            "Confirm the main contact",
        );
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(setup.otherUserProjectId, taskPath);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
