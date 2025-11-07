import { ProjectBuilder } from "cypress/api/projectBuilder";
import externalContactsPage from "cypress/pages/projects/projectDetails/externalContactsPage";
import externalContactsAddPage from "cypress/pages/projects/projectDetails/externalContactsAddPage";
import { macclesfieldTrust, yesNoOption } from "cypress/constants/stringTestConstants";
import { Logger } from "cypress/common/logger";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { urnPool } from "cypress/constants/testUrns";

const incomingTrust = macclesfieldTrust;
const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversion.stChads
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
        projectRemover.removeProjectIfItExists(project.urn);
        projectApi.createConversionProject(project).then((response) => (projectId = response.value));
    });
    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/external-contacts`);
    });
    for (const contact of contacts) {
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
    }

    it("Add headteacher contact type", () => {
        Logger.log("Add Headteacher contact");
        externalContactsPage.clickButton("Add contact");

        externalContactsAddPage
            .selectHeadteacher()
            .saveAndContinue()
            .withName("Mr Headteacher")
            .withEmail("headteacher@test.com")
            .withPersonIsPrimaryContact("No")
            .saveAndContinue();

        Logger.log("Check headteacher contact");
        externalContactsPage
            .containsSuccessBannerWithMessage("Contact added")
            .setContactItemByRoleHeading("Headteacher")
            .hasContactItem()
            .summaryShows("Name")
            .hasValue("Mr Headteacher")
            .summaryShows("Email")
            .hasValue("headteacher@test.com")
            .summaryShows("Primary contact at organisation?")
            .hasValue("No");
    });

    it("Add incoming trust CEO contact type", () => {
        Logger.log("Add Incoming trust CEO contact");
        externalContactsPage.clickButton("Add contact");

        externalContactsAddPage
            .selectIncomingTrustCEO()
            .saveAndContinue()
            .withName("Ms CEO")
            .withEmail("ceo@test.com")
            .withPhone("01234567894")
            .withPersonIsPrimaryContact("Yes")
            .saveAndContinue();

        Logger.log("Check incoming trust CEO contact");

        externalContactsPage
            .containsSuccessBannerWithMessage("Contact added")
            .setContactItemByRoleHeading("CEO")
            .hasContactItem()
            .summaryShows("Name")
            .hasValue("Ms CEO")
            .summaryShows("Email")
            .hasValue("ceo@test.com")
            .summaryShows("Phone")
            .hasValue("01234567894")
            .summaryShows("Primary contact at organisation?")
            .hasValue("Yes");
    });

    it("Add chair of governors contact type", () => {
        Logger.log("Add Chair of governors");
        externalContactsPage.clickButton("Add contact");
        externalContactsAddPage
            .selectChairOfGovernors()
            .saveAndContinue()
            .withName("Mrs Chair")
            .withEmail("chair@test.com")
            .saveAndContinue();

        Logger.log("Check chair of governors contact");
        externalContactsPage
            .containsSuccessBannerWithMessage("Contact added")
            .setContactItemByRoleHeading("Chair of governors")
            .hasContactItem()
            .summaryShows("Name")
            .hasValue("Mrs Chair")
            .summaryShows("Email")
            .hasValue("chair@test.com")
            .summaryShows("Primary contact at organisation?")
            .hasValue("No");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
