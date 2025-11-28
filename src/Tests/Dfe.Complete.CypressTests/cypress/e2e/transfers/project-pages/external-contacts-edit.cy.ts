import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import contactApi from "cypress/api/contactApi";
import { ContactBuilder } from "cypress/api/contactBuilder";
import externalContactsPage from "cypress/pages/projects/projectDetails/externalContactsPage";
import externalContactsEditPage from "cypress/pages/projects/projectDetails/externalContactsEditPage";
import { Logger } from "cypress/common/logger";
import { ContactCategory } from "cypress/api/apiDomain";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transfer.abbey,
});
let projectId: string;
const schoolName = "Abbey Primary School";
const schoolLocalAuthority = "Manchester City Council";
const contact = ContactBuilder.createContactRequest({
    fullName: "Testy McTestface",
    organisationName: schoolName,
});
const contactToDelete = ContactBuilder.createContactRequest({
    fullName: "Delete Me",
    role: "Director of Education",
    category: ContactCategory.LocalAuthority,
    organisationName: schoolLocalAuthority,
    isPrimaryContact: true,
});

describe("Edit and delete external contacts tests:", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectApi.createAndUpdateTransferProject(project).then((response) => {
            projectId = response.value;
            contact.projectId = { value: projectId };
            contactApi.createContact(contact);
            contactToDelete.projectId = { value: projectId };
            contactApi.createContact(contactToDelete);
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/external-contacts`);
    });

    it("Should be able to edit an existing external contact", () => {
        const editedContact = {
            name: "Edited Name",
            role: "Edited Role",
            email: "edited@test.com",
            phone: "09999999999",
            isPrimaryContact: false,
            organisation: "Solicitor",
            organisationName: "Solicitor McSolicitorface Ltd",
        };

        Logger.log("Edit the contact just created");
        externalContactsPage.setContactItemByRoleHeading(contact.role!).hasContactItem().editContact();

        externalContactsEditPage
            .withName(editedContact.name)
            .withRole(editedContact.role)
            .withEmail(editedContact.email)
            .withPhone(editedContact.phone)
            .withOrganisation(editedContact.organisation)
            .withOrganisationName(editedContact.organisationName)
            .withPersonIsPrimaryContact(editedContact.isPrimaryContact ? "Yes" : "No")
            .saveContact();

        Logger.log("Check contact details have been updated");
        externalContactsPage
            .containsSuccessBannerWithMessage("Contact updated")
            .containsOrganisationHeading("Solicitors contacts")
            .setContactItemByRoleHeading(editedContact.role)
            .hasContactItem()
            .summaryShows("Name")
            .hasValue(editedContact.name)
            .summaryShows("Email")
            .hasValue(editedContact.email)
            .summaryShows("Phone")
            .hasValue(editedContact.phone);
    });

    it("Should be able to delete an existing external contact", () => {
        Logger.log("Delete contact");
        externalContactsPage.setContactItemByRoleHeading(contactToDelete.role!).hasContactItem().editContact();

        externalContactsEditPage
            .deleteContact()
            .contains(`Are you sure you want to delete ${contactToDelete.fullName}?`)
            .deleteContact();

        Logger.log("Check contact has been deleted");
        externalContactsPage.containsSuccessBannerWithMessage("Contact deleted");
        externalContactsPage.doesntContain(contactToDelete.fullName!);
    });
});
