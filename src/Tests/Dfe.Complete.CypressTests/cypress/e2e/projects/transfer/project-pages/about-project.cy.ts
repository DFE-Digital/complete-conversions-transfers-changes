import { Logger } from "cypress/common/logger";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { companiesHouseUrl, dimensionsTrust, giasUrl, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { significateDateToDisplayDate } from "cypress/support/formatDate";
import editTransferProjectPage from "cypress/pages/projects/edit/editTransferProjectPage";
import groupApi from "cypress/api/groupApi";

const project = ProjectBuilder.createTransferProjectRequest({
    incomingTrustUkprn: { value: macclesfieldTrust.ukprn },
    outgoingTrustUkprn: { value: dimensionsTrust.ukprn },
});
let projectId: string;
let changeLinkPath: string;
const schoolName = "Abbey College Manchester";
const localAuthority = "Manchester";
const region = "North West";
const incomingTrust = macclesfieldTrust;
const outgoingTrust = dimensionsTrust;

const projectFormAMat = ProjectBuilder.createTransferFormAMatProjectRequest({
    newTrustName: macclesfieldTrust.name,
    newTrustReferenceNumber: macclesfieldTrust.referenceNumber,
    outgoingTrustUkprn: { value: dimensionsTrust.ukprn },
});
let projectFormAMatId: string;
let formAMATChangeLinkPath: string;
let groupId: string;
const formAMATSchoolName = "Priory Rise School";
const formAMATLocalAuthority = "Milton Keynes";
const formAMATRegion = "South East";

describe("About the project page - transfer projects: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${projectFormAMat.urn.value}`);
        projectApi.createTransferProject(project).then((response) => {
            projectId = response.value;
            changeLinkPath = `/projects/transfers/${projectId}/edit#`;
        });
        projectApi.createMatTransferProject(projectFormAMat).then((response) => {
            projectFormAMatId = response.value;
            formAMATChangeLinkPath = `/projects/transfers/${projectFormAMatId}/edit#`;
        });
        groupApi
            .getGroupBy("groupIdentifier", dimensionsTrust.groupReferenceNumber)
            .then((groups) => (groupId = groups[0].groupId));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should display the project details on the about project section for a transfer project", () => {
        Logger.log("Go to project");
        cy.visit(`projects/${projectId}/tasks`);

        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Project summary details are still displayed at the top of the page");
        projectDetailsPage
            .containsHeading(schoolName)
            .hasAcademyURNHeading(`${project.urn.value}`)
            .hasTransferTag()
            .hasInAGroupTag()
            .hasTransferDate(project.significantDate)
            .hasOutgoingTrust(outgoingTrust.name)
            .hasIncomingTrust(incomingTrust.name)
            .hasLAAndRegion(localAuthority, region)
            .hasSharePointLink(project.establishmentSharepointLink);

        Logger.log("About projects sections are displayed as expected");

        aboutTheProjectPage
            .inOrder()
            .subSection("Project details")
            .hasSubHeading("Project details")
            .summaryShows("Type")
            .hasValue("Transfer")
            .summaryShows("Transfer date")
            .hasValue(significateDateToDisplayDate(project.significantDate))
            .summaryShows("Local authority")
            .hasValue(localAuthority)
            .summaryShows("Diocese")
            .hasValue("Not applicable")
            .summaryShows("Region")
            .hasValue(region)
            .summaryShows("Group reference number")
            .hasValue(project.groupReferenceNumber!)
            .hasChangeLink(`${changeLinkPath}group-reference-number`)

            .subSection("Project assignment")
            .hasSubHeading("Project assignment")
            .summaryShows("Are you handing this project over to RCS (Regional Casework Services)?")
            .hasValue(project.handingOverToRegionalCaseworkService ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}project-assignment`)

            .subSection("Reasons for the transfer")
            .hasSubHeading("Reasons for the transfer")
            .summaryShows("Is this transfer due to 2RI?")
            .hasValue(project.isDueTo2Ri ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}two-requires-improvement`)
            .summaryShows("Is this transfer due to an inadequate Ofsted rating?")
            .hasValue(project.isDueToInedaquateOfstedRating ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}inadequate-ofsted`)
            .summaryShows("Is this transfer due to financial, safeguarding or governance issues?")
            .hasValue(project.isDueToIssues ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}financial-safeguarding-governance-issues`)

            .subSection("Advisory board details")
            .hasSubHeading("Advisory board details")
            .summaryShows("Date of advisory board")
            .hasValue(significateDateToDisplayDate(project.advisoryBoardDate))
            .hasChangeLink(`${changeLinkPath}advisory-board`)
            .summaryShows("Conditions from advisory board")
            .hasValue(project.advisoryBoardConditions)
            .hasChangeLink(`${changeLinkPath}advisory-board`)

            .subSection("Academy details")
            .hasSubHeading("Academy details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${schoolName} View the school's information in GIAS (opens in new tab)`,
                `${giasUrl}/Establishments/Establishment/Details/${project.urn.value}`,
            )
            .summaryShows("Academy URN (unique reference number)")
            .hasValue(project.urn.value)
            .summaryShows("Type")
            .hasValue("Other independent school")
            .summaryShows("Age range")
            .hasValue("14 to 23")
            .summaryShows("Phase")
            .hasValue("Not applicable")
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the academy SharePoint folder (opens in new tab)",
                project.establishmentSharepointLink,
            )
            .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`)

            .subSection("Incoming trust details")
            .hasSubHeading("Incoming trust details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${incomingTrust.name.toUpperCase()} View the trust information in GIAS (opens in new tab)`,
                `${giasUrl}/Groups/Search?GroupSearchModel.Text=${incomingTrust.ukprn}`,
            )
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue(incomingTrust.ukprn)
            // .hasChangeLink(`${changeLinkPath}incoming-trust-ukprn`)
            .summaryShows("Group ID (identifier)")
            .hasValue(incomingTrust.referenceNumber)
            .summaryShows("Companies House number")
            .hasValueWithLink(
                `${incomingTrust.companiesHouseNumber} View the Companies House information (opens in new tab)`,
                `${companiesHouseUrl}${incomingTrust.companiesHouseNumber}`,
            )
            .summaryShows("New trust reference number (TRN)")
            .hasValue("")
            .summaryShows("Address")
            .hasValue(incomingTrust.address)
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the trust SharePoint folder (opens in new tab)",
                project.incomingTrustSharepointLink,
            )
            .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`)

            .subSection("Outgoing trust details")
            .hasSubHeading("Outgoing trust details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${outgoingTrust.name.toUpperCase()} View the trust information in GIAS (opens in new tab)`,
                `${giasUrl}/Groups/Search?GroupSearchModel.Text=${outgoingTrust.ukprn}`,
            )
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue(outgoingTrust.ukprn)
            .hasChangeLink(`${changeLinkPath}outgoing-trust-ukprn`)
            .summaryShows("Group ID (identifier)")
            .hasValue(outgoingTrust.referenceNumber)
            .summaryShows("Companies House number")
            .hasValueWithLink(
                `${outgoingTrust.companiesHouseNumber} View the Companies House information (opens in new tab)`,
                `${companiesHouseUrl}${outgoingTrust.companiesHouseNumber}`,
            )
            .summaryShows("Address")
            .hasValue(outgoingTrust.address)
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the trust SharePoint folder (opens in new tab)",
                project.outgoingTrustSharepointLink,
            )
            .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`)
            .summaryShows("Will the outgoing trust close once this transfer is completed?")
            .hasValue(project.outGoingTrustWillClose ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}outgoing-trust-to-close`);
    });

    it("Should display the project details on the about project section for a transfer form a MAT project", () => {
        Logger.log("Go to the about project section");
        cy.visit(`projects/${projectFormAMatId}/information`);

        Logger.log("Project summary details are still displayed at the top of the page");
        projectDetailsPage
            .containsHeading(formAMATSchoolName)
            .hasAcademyURNHeading(`${projectFormAMat.urn.value}`)
            .hasTransferTag()
            .hasFormAMATTag()
            .hasTransferDate(projectFormAMat.significantDate)
            .hasOutgoingTrust(outgoingTrust.name)
            .hasIncomingTrust(incomingTrust.name)
            .hasLAAndRegion(formAMATLocalAuthority, formAMATRegion)
            .hasSharePointLink(projectFormAMat.establishmentSharepointLink);

        Logger.log("About projects details sections are displayed as expected");
        aboutTheProjectPage
            .inOrder()
            .subSection("Project details")
            .hasSubHeading("Project details")
            .summaryShows("Type")
            .hasValue("Transfer")
            .summaryShows("Transfer date")
            .hasValue(significateDateToDisplayDate(project.significantDate))
            .summaryShows("Local authority")
            .hasValue(formAMATLocalAuthority)
            .summaryShows("Diocese")
            .hasValue("Not applicable")
            .summaryShows("Region")
            .hasValue(formAMATRegion)
            .summaryShows("Group reference number")
            .hasValue("Not grouped")
            .hasChangeLink(`${formAMATChangeLinkPath}group-reference-number`)

            .subSection("Project assignment")
            .hasSubHeading("Project assignment")
            .summaryShows("Are you handing this project over to RCS (Regional Casework Services)?")
            .hasValue(projectFormAMat.handingOverToRegionalCaseworkService ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}project-assignment`)

            .subSection("Reasons for the transfer")
            .hasSubHeading("Reasons for the transfer")
            .summaryShows("Is this transfer due to 2RI?")
            .hasValue(projectFormAMat.isDueTo2Ri ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}two-requires-improvement`)
            .summaryShows("Is this transfer due to an inadequate Ofsted rating?")
            .hasValue(projectFormAMat.isDueToInedaquateOfstedRating ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}inadequate-ofsted`)
            .summaryShows("Is this transfer due to financial, safeguarding or governance issues?")
            .hasValue(projectFormAMat.isDueToIssues ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}financial-safeguarding-governance-issues`)

            .subSection("Advisory board details")
            .hasSubHeading("Advisory board details")
            .summaryShows("Date of advisory board")
            .hasValue(significateDateToDisplayDate(projectFormAMat.advisoryBoardDate))
            .hasChangeLink(`${formAMATChangeLinkPath}advisory-board`)
            .summaryShows("Conditions from advisory board")
            .hasValue(projectFormAMat.advisoryBoardConditions)
            .hasChangeLink(`${formAMATChangeLinkPath}advisory-board`)

            .subSection("Academy details")
            .hasSubHeading("Academy details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${formAMATSchoolName} View the school's information in GIAS (opens in new tab)`,
                `${giasUrl}/Establishments/Establishment/Details/${projectFormAMat.urn.value}`,
            )
            .summaryShows("Academy URN (unique reference number)")
            .hasValue(projectFormAMat.urn.value)
            .summaryShows("Type")
            .hasValue("Academy converter")
            .summaryShows("Age range")
            .hasValue("3 to 11")
            .summaryShows("Phase")
            .hasValue("Primary")
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the academy SharePoint folder (opens in new tab)",
                projectFormAMat.establishmentSharepointLink,
            )
            .hasChangeLink(`${formAMATChangeLinkPath}sharepoint-folder-links`)

            .subSection("Incoming trust details")
            .hasSubHeading("Incoming trust details")
            .summaryShows("Name")
            .hasValueWithLink(incomingTrust.name)
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue("")
            .hasChangeLink(`${formAMATChangeLinkPath}incoming-trust-ukprn`)
            .summaryShows("Group ID (identifier)")
            .hasValue(incomingTrust.referenceNumber)
            .summaryShows("Companies House number")
            .hasValueWithLink("")
            .summaryShows("New trust reference number (TRN)")
            .hasValue(projectFormAMat.newTrustReferenceNumber)
            .summaryShows("Address")
            .hasValue("")
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the trust SharePoint folder (opens in new tab)",
                projectFormAMat.incomingTrustSharepointLink,
            )
            .hasChangeLink(`${formAMATChangeLinkPath}sharepoint-folder-links`)

            .subSection("Outgoing trust details")
            .hasSubHeading("Outgoing trust details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${outgoingTrust.name.toUpperCase()} View the trust information in GIAS (opens in new tab)`,
                `${giasUrl}/Groups/Search?GroupSearchModel.Text=${outgoingTrust.ukprn}`,
            )
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue(outgoingTrust.ukprn)
            .hasChangeLink(`${formAMATChangeLinkPath}outgoing-trust-ukprn`)
            .summaryShows("Group ID (identifier)")
            .hasValue(outgoingTrust.referenceNumber)
            .summaryShows("Companies House number")
            .hasValueWithLink(
                `${outgoingTrust.companiesHouseNumber} View the Companies House information (opens in new tab)`,
                `${companiesHouseUrl}${outgoingTrust.companiesHouseNumber}`,
            )
            .summaryShows("Address")
            .hasValue(outgoingTrust.address)
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the trust SharePoint folder (opens in new tab)",
                project.outgoingTrustSharepointLink,
            )
            .hasChangeLink(`${formAMATChangeLinkPath}sharepoint-folder-links`)
            .summaryShows("Will the outgoing trust close once this transfer is completed?")
            .hasValue(project.outGoingTrustWillClose ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}outgoing-trust-to-close`);
    });

    it("Should display page links that navigate to different sections of the about project page", () => {
        Logger.log("Go to the about project section");
        cy.visit(`projects/${projectId}/information`);

        Logger.log("Check that the page links correctly navigate to the different sections");
        aboutTheProjectPage
            .jumpToSection("Project details")
            .pageHasMovedToSection("Project details")
            .jumpToSection("Project assignment")
            .pageHasMovedToSection("Project assignment")
            .jumpToSection("Reasons for the transfer")
            .pageHasMovedToSection("Reasons for the transfer")
            .jumpToSection("Advisory board details")
            .pageHasMovedToSection("Advisory board details")
            .jumpToSection("Academy details")
            .pageHasMovedToSection("Academy details")
            .jumpToSection("Incoming trust details")
            .pageHasMovedToSection("Incoming trust details")
            .jumpToSection("Outgoing trust details")
            .pageHasMovedToSection("Outgoing trust details");
    });

    it("Should be able to make changes to your project's details", () => {
        const newIncomingTrust = dimensionsTrust;
        const newOutgoingTrust = macclesfieldTrust;
        cy.visit(`projects/${projectId}/information`);

        Logger.log("Go to change form");
        aboutTheProjectPage.change("Group reference number");

        Logger.log("Update project details");
        editTransferProjectPage
            .withOutgoingTrustUKPRN(newOutgoingTrust.ukprn)
            .withIncomingTrustUKPRN(newIncomingTrust.ukprn)
            .withTrustReferenceNumber("")
            .withGroupReferenceNumber(newIncomingTrust.groupReferenceNumber)
            .withAdvisoryBoardDate("10", "01", "2024")
            .withAdvisoryBoardConditions("Updated advisory board conditions")
            .withSchoolOrAcademySharePointLink("https://educationgovuk.sharepoint.com/11")
            .withIncomingTrustSharePointLink("https://educationgovuk.sharepoint.com/22")
            .withOutgoingTrustSharePointLink("https://educationgovuk.sharepoint.com/33")
            .with2RI("Yes")
            .withTransferDueToInadequateOfstedRating("Yes")
            .withTransferDueToFinancialSafeguardingGovernanceIssues("Yes")
            .withOutgoingTrustWillCloseAfterTransfer("Yes")
            .withHandingOverToRCS("Yes")
            .withHandoverComments("Updated handover comments")
            .continue();

        aboutTheProjectPage
            .containsSuccessBannerWithMessage("Project has been updated successfully")
            .subSection("Project details")
            .keyHasValueWithLink("Group reference number", newIncomingTrust.groupReferenceNumber, `/groups/${groupId}`)
            .subSection("Project assignment")
            .keyHasValue("Are you handing this project over to RCS (Regional Casework Services)?", "Yes")
            .subSection("Reasons for the transfer")
            .keyHasValue("Is this transfer due to 2RI?", "Yes")
            .keyHasValue("Is this transfer due to an inadequate Ofsted rating?", "Yes")
            .keyHasValue("Is this transfer due to financial, safeguarding or governance issues?", "Yes")
            .subSection("Advisory board details")
            .keyHasValue("Date of advisory board", "10 January 2024")
            .keyHasValue("Conditions from advisory board", "Updated advisory board conditions")
            .subSection("Academy details")
            .keyHasValueWithLink(
                "SharePoint folder",
                "View the academy SharePoint folder (opens in new tab)",
                "https://educationgovuk.sharepoint.com/11",
            )
            .subSection("Incoming trust details")
            .keyHasValueWithLink(
                "Name",
                `${newIncomingTrust.name.toUpperCase()} View the trust information in GIAS (opens in new tab)`,
                `${giasUrl}/Groups/Search?GroupSearchModel.Text=${newIncomingTrust.ukprn}`,
            )
            .keyHasValue("UKPRN (UK provider reference number)", newIncomingTrust.ukprn)
            .keyHasValue("Group ID (identifier)", newIncomingTrust.referenceNumber)
            .keyHasValueWithLink(
                "Companies House number",
                `${newIncomingTrust.companiesHouseNumber} View the Companies House information (opens in new tab)`,
                `${companiesHouseUrl}${newIncomingTrust.companiesHouseNumber}`,
            )
            .keyHasValue("New trust reference number (TRN)", "")
            .keyHasValue("Address", newIncomingTrust.address)
            .keyHasValueWithLink(
                "SharePoint folder",
                "View the trust SharePoint folder (opens in new tab)",
                "https://educationgovuk.sharepoint.com/22",
            )
            .subSection("Outgoing trust details")
            .keyHasValueWithLink(
                "Name",
                `${newOutgoingTrust.name.toUpperCase()} View the trust information in GIAS (opens in new tab)`,
                `${giasUrl}/Groups/Search?GroupSearchModel.Text=${newOutgoingTrust.ukprn}`,
            )
            .keyHasValue("UKPRN (UK provider reference number)", newOutgoingTrust.ukprn)
            .keyHasValue("Group ID (identifier)", newOutgoingTrust.referenceNumber)
            .keyHasValueWithLink(
                "Companies House number",
                `${newOutgoingTrust.companiesHouseNumber} View the Companies House information (opens in new tab)`,
                `${companiesHouseUrl}${newOutgoingTrust.companiesHouseNumber}`,
            )
            .keyHasValue("Address", newOutgoingTrust.address)
            .keyHasValueWithLink(
                "SharePoint folder",
                "View the trust SharePoint folder (opens in new tab)",
                "https://educationgovuk.sharepoint.com/33",
            )
            .keyHasValue("Will the outgoing trust close once this transfer is completed?", "Yes");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
