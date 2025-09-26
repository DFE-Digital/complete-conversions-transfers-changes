import { ProjectBuilder } from "cypress/api/projectBuilder";
import externalContactsPage from "cypress/pages/projects/projectDetails/externalContactsPage";
import externalContactsAddPage from "cypress/pages/projects/projectDetails/externalContactsAddPage";
import { macclesfieldTrust, yesNoOption } from "cypress/constants/stringTestConstants";
import { Logger } from "cypress/common/logger";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const incomingTrust = macclesfieldTrust;
const project = ProjectBuilder.createConversionProjectRequest({
    incomingTrustUkprn: { value: incomingTrust.ukprn },
});
let projectId: string;
const schoolName = "St Chad's Catholic Primary School";
const localAuthority = "Dudley Metropolitan Borough Council";

const contacts: {
    index: number;
    name: string;
    role: string;
    email: string;
    phone: string;
    organisation: string;
    organisationName: string;
    primaryContact: yesNoOption;
}[] = [
    {
        index: 0,
        name: "Test Executive Headteacher",
        role: "Executive Headteacher",
        email: "execheadteacher@test.com",
        phone: "01234567890",
        organisation: "School or academy",
        organisationName: schoolName,
        primaryContact: "No",
    },
    {
        index: 1,
        name: "Test Deputy CEO",
        role: "Deputy CEO",
        email: "deputyceo@test.com",
        phone: "01234567891",
        organisation: "Incoming trust",
        organisationName: incomingTrust.name,
        primaryContact: "Yes",
    },
    {
        index: 2,
        name: "Test Director of Education",
        role: "Director of Education",
        email: "education@test.com",
        phone: "01234567892",
        organisation: "Local authority",
        organisationName: localAuthority,
        primaryContact: "No",
    },
    {
        index: 4,
        name: "Test Solicitor Partner",
        role: "Partner",
        email: "partner@test.com",
        phone: "01234567893",
        organisation: "Solicitor",
        organisationName: "Solicitors",
        primaryContact: "No",
    },
    {
        index: 5,
        name: "Test Deputy Director of Schools",
        role: "Deputy Director of Schools",
        email: "deputydirectorschools@test.com",
        phone: "01234567894",
        organisation: "Diocese",
        organisationName: "Diocese",
        primaryContact: "No",
    },
    {
        index: 6,
        name: "Test Project manager",
        role: "Project manager",
        email: "projectmanager@test.com",
        phone: "01234567895",
        organisation: "Other",
        organisationName: "Other",
        primaryContact: "No",
    },
];

describe("Add external contacts tests:", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createConversionProject(project).then((response) => (projectId = response.value));
    });
    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/external-contacts`);
    });
    contacts.forEach((contact) => {
        it(`Add someone else - '${contact.organisation}' contact type`, () => {
            Logger.log(`Add '${contact.organisation}' contact`);

            externalContactsPage.clickButton("Add contact");

            externalContactsAddPage
                .selectSomeoneElse()
                .saveAndContinue()
                .withName(contact.name)
                .withRole(contact.role)
                .withEmail(contact.email)
                .withPhone(contact.phone)
                .withOrganisation(contact.organisation)
                .withPersonIsPrimaryContact(contact.primaryContact)
                .saveAndContinue();

            Logger.log("Check contact details have been added");
            const page = externalContactsPage
                .containsSuccessBannerWithMessage("Contact added")
                .containsOrganisationHeading(`${contact.organisationName} contacts`)
                .setContactItem(contact.index)
                .hasContactItem()
                .hasRoleHeading(contact.role)
                .summaryShows("Name")
                .hasValue(contact.name)
                .summaryShows("Email")
                .hasValue(contact.email)
                .summaryShows("Phone")
                .hasValue(contact.phone);

            if (["School or academy", "Incoming trust", "Local Authority"].includes(contact.organisation)) {
                page.summaryShows("Primary contact at organisation?").hasValue(contact.primaryContact);
            }
        });
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
