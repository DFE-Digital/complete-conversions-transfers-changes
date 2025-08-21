import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { dimensionsTrust, giasUrl, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { significateDateToDisplayDate } from "cypress/support/formatDate";
import groupApi from "cypress/api/groupApi";

const project = ProjectBuilder.createConversionProjectRequest({
    incomingTrustUkprn: { value: dimensionsTrust.ukprn },
    groupReferenceNumber: dimensionsTrust.groupReferenceNumber,
});
let projectId: string;
let changeLinkPath: string;
const schoolName = "St Chad's Catholic Primary School";
const region = "West Midlands";
const localAuthority = "Dudley";
const academy = {
    urn: 103846,
    name: "Cradley CofE Primary School",
    address: "Church Road",
    localAuthority: "Dudley",
    schoolPhase: "Primary",
    sharePointLink: "https://educationgovuk.sharepoint.com",
};

const projectFormAMAT = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectFormAMATId: string;
const formAMATSchoolName = "Whitchurch Primary School";
const formAMATLocalAuthority = "Bath and North East Somerset";
const formAMATRegion = "South West";

const teammatesProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: 147801 },
    userAdId: rdoLondonUser.adId,
});
let teammatesProjectId: string;
let formAMATChangeLinkPath: string;
let groupId: string;
describe("About the project page - conversion projects: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${projectFormAMAT.urn.value}`);
        projectRemover.removeProjectIfItExists(`${teammatesProject.urn.value}`);
        projectApi.createConversionProject(project).then((response) => {
            projectId = response.value;
            projectApi.updateProjectAcademyUrn(projectId, academy.urn);
            changeLinkPath = `/projects/conversions/${projectId}/edit#`;
        });
        projectApi.createMatConversionProject(projectFormAMAT).then((response) => {
            projectFormAMATId = response.value;
            formAMATChangeLinkPath = `/projects/conversions/${projectFormAMATId}/edit#`;
        });
        projectApi
            .createMatConversionProject(teammatesProject, rdoLondonUser.email)
            .then((response) => (teammatesProjectId = response.value));
        groupApi.getGroupBy("groupIdentifier", dimensionsTrust.groupReferenceNumber).then((groups) => {
            if (groups.length > 0) {
                groupId = groups[0].groupId;
            } else {
                throw new Error("Group does not exist");
            }
        });
    });
    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectFormAMATId}/tasks`);
    });

    it("Should display the project details on the about the project section for a Conversion project", () => {
        cy.visit(`/projects/${projectId}/tasks`);
        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Project summary details are still displayed at the top of the page");
        projectDetailsPage
            .containsHeading(schoolName)
            .hasSchoolURNHeading(`${project.urn.value}`)
            .hasConversionTag()
            .hasInAGroupTag()
            .hasConversionDate(project.significantDate)
            .hasIncomingTrust(dimensionsTrust.name)
            .hasLAAndRegion(localAuthority, region)
            .hasSharePointLinks(project.establishmentSharepointLink, project.incomingTrustSharepointLink);

        Logger.log("Project details sections are displayed as expected");

        aboutTheProjectPage
            .inOrder()
            .subSection("Project details")
            .hasSubHeading("Project details")
            .summaryShows("Type")
            .hasValue("Conversion")
            .summaryShows("Conversion date")
            .hasValue(significateDateToDisplayDate(project.significantDate))
            .summaryShows("Local authority")
            .hasValue(localAuthority)
            .summaryShows("Diocese")
            .hasValue("Archdiocese of Birmingham")
            .summaryShows("Region")
            .hasValue(region)
            .summaryShows("Group reference number")
            .hasValueWithLink(dimensionsTrust.groupReferenceNumber, `/groups/${groupId}`)
            .hasChangeLink(`${changeLinkPath}group-reference-number`)

            .subSection("Project assignment")
            .hasSubHeading("Project assignment")
            .summaryShows("Are you handing this project over to RCS (Regional Casework Services)?")
            .hasValue(project.handingOverToRegionalCaseworkService ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}project-assignment`)

            .subSection("Reasons for the conversion")
            .hasSubHeading("Reasons for the conversion")
            .summaryShows("Has a directive academy order been issued?")
            .hasValue(project.hasAcademyOrderBeenIssued ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}directive-academy-order`)
            .summaryShows("Is this conversion due to intervention following 2RI?")
            .hasValue(project.isDueTo2Ri ? "Yes" : "No")
            .hasChangeLink(`${changeLinkPath}two-requires-improvement`)

            .subSection("Advisory board details")
            .hasSubHeading("Advisory board details")
            .summaryShows("Date of advisory board")
            .hasValue(significateDateToDisplayDate(project.advisoryBoardDate))
            .hasChangeLink(`${changeLinkPath}advisory-board`)
            .summaryShows("Conditions from advisory board")
            .hasValue(project.advisoryBoardConditions)
            .hasChangeLink(`${changeLinkPath}advisory-board`)

            .subSection("School details")
            .hasSubHeading("School details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${schoolName} View the school's information in GIAS (opens in new tab)`,
                `${giasUrl}/Establishments/Establishment/Details/${project.urn.value}`,
            )
            .summaryShows("URN (unique reference number)")
            .hasValue(project.urn.value)
            .summaryShows("Type")
            .hasValue("Voluntary aided school")
            .summaryShows("Range")
            .hasValue("4 to 11")
            .summaryShows("Phase")
            .hasValue("Primary")
            .summaryShows("Address")
            .hasValue("Catholic Lane Dudley West Midlands DY3 3UE")
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the school SharePoint folder (opens in new tab)",
                project.establishmentSharepointLink,
            )
            .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`)

            .subSection("Academy details")
            .hasSubHeading("Academy details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${academy.name} View the school's information in GIAS (opens in new tab)`,
                `${giasUrl}/Establishments/Establishment/Details/${academy.urn}`,
            )
            .summaryShows("Academy URN (unique reference number)")
            .hasValue(academy.urn)
            .summaryShows("LAESTAB (DfE number)")
            .hasValue("332/3350")
            .summaryShows("Type")
            .hasValue("Voluntary aided school")
            .summaryShows("Age range")
            .hasValue("4 to 11")
            .summaryShows("Phase")
            .hasValue("Primary")
            .summaryShows("SharePoint folder")
            .hasValueWithLink("View the academy SharePoint folder (opens in new tab)", academy.sharePointLink)

            .subSection("Incoming trust details")
            .hasSubHeading("Incoming trust details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${dimensionsTrust.name.toUpperCase()} View the trust information in GIAS (opens in new tab)`,
                `${giasUrl}/Groups/Search?GroupSearchModel.Text=${dimensionsTrust.ukprn}`,
            )
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue(dimensionsTrust.ukprn)
            .hasChangeLink(`${changeLinkPath}incoming-trust-ukprn`)
            .summaryShows("Group ID (identifier)")
            .hasValue(dimensionsTrust.referenceNumber)
            .summaryShows("Companies House number")
            .hasValueWithLink(
                `${dimensionsTrust.companiesHouseNumber} View the Companies House information (opens in new tab)`,
                `https://find-and-update.company-information.service.gov.uk/company/${dimensionsTrust.companiesHouseNumber}`,
            )
            .summaryShows("New trust reference number (TRN)")
            .hasValue("")
            .summaryShows("Address")
            .hasValue(dimensionsTrust.address)
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the trust SharePoint folder (opens in new tab)",
                project.incomingTrustSharepointLink,
            )
            .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`);
    });

    it("Should display the project details on the about project section for Conversion form a MAT project", () => {
        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Project summary details are still displayed at the top of the page");
        projectDetailsPage
            .containsHeading(formAMATSchoolName)
            .hasSchoolURNHeading(`${projectFormAMAT.urn.value}`)
            .hasFormAMATTag()
            .hasConversionTag()
            .hasConversionDate(projectFormAMAT.significantDate)
            .hasIncomingTrust(macclesfieldTrust.name)
            .hasLAAndRegion("Bath And North East Somerset", formAMATRegion)
            .hasSharePointLinks(
                projectFormAMAT.establishmentSharepointLink,
                projectFormAMAT.incomingTrustSharepointLink,
            );

        Logger.log("Project details sections are displayed as expected");

        aboutTheProjectPage
            .inOrder()
            .subSection("Project details")
            .hasSubHeading("Project details")
            .summaryShows("Type")
            .hasValue("Conversion")
            .summaryShows("Conversion date")
            .hasValue(significateDateToDisplayDate(projectFormAMAT.significantDate))
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
            .hasValue(projectFormAMAT.handingOverToRegionalCaseworkService ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}project-assignment`)

            .subSection("Reasons for the conversion")
            .hasSubHeading("Reasons for the conversion")
            .summaryShows("Has a directive academy order been issued?")
            .hasValue(projectFormAMAT.hasAcademyOrderBeenIssued ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}directive-academy-order`)
            .summaryShows("Is this conversion due to intervention following 2RI?")
            .hasValue(projectFormAMAT.isDueTo2Ri ? "Yes" : "No")
            .hasChangeLink(`${formAMATChangeLinkPath}two-requires-improvement`)

            .subSection("Advisory board details")
            .hasSubHeading("Advisory board details")
            .summaryShows("Date of advisory board")
            .hasValue(significateDateToDisplayDate(projectFormAMAT.advisoryBoardDate))
            .hasChangeLink(`${formAMATChangeLinkPath}advisory-board`)
            .summaryShows("Conditions from advisory board")
            .hasValue(projectFormAMAT.advisoryBoardConditions)
            .hasChangeLink(`${formAMATChangeLinkPath}advisory-board`)

            .subSection("School details")
            .hasSubHeading("School details")
            .summaryShows("Name")
            .hasValueWithLink(
                `${formAMATSchoolName} View the school's information in GIAS (opens in new tab)`,
                `${giasUrl}/Establishments/Establishment/Details/${projectFormAMAT.urn.value}`,
            )
            .summaryShows("URN (unique reference number)")
            .hasValue(projectFormAMAT.urn.value)
            .summaryShows("Type")
            .hasValue("Academy converter")
            .summaryShows("Range")
            .hasValue("4 to 11")
            .summaryShows("Phase")
            .hasValue("Primary")
            .summaryShows("Address")
            .hasValue("22 Bristol Road Bristol BS14 0PT")
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the school SharePoint folder (opens in new tab)",
                projectFormAMAT.establishmentSharepointLink,
            )
            .hasChangeLink(`${formAMATChangeLinkPath}sharepoint-folder-links`)

            .subSection("Academy details")
            .hasSubHeading("Academy details")
            .hasNoAcademyDetailsAsURNNotProvided()

            .subSection("Incoming trust details")
            .hasSubHeading("Incoming trust details")
            .summaryShows("Name")
            .hasValue(macclesfieldTrust.name)
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue("")
            .hasChangeLink(`${formAMATChangeLinkPath}incoming-trust-ukprn`)
            .summaryShows("Group ID (identifier)")
            .hasValue(macclesfieldTrust.referenceNumber)
            .summaryShows("Companies House number")
            .hasValue("")
            .summaryShows("New trust reference number (TRN)")
            .hasValue(projectFormAMAT.newTrustReferenceNumber)
            .summaryShows("Address")
            .hasValue("")
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the trust SharePoint folder (opens in new tab)",
                projectFormAMAT.incomingTrustSharepointLink,
            )
            .hasChangeLink(`${formAMATChangeLinkPath}sharepoint-folder-links`);
    });

    it("Should display page links that navigate to different sections of the about project page", () => {
        Logger.log("Go to the about project section");
        cy.visit(`projects/${projectFormAMATId}/information`);

        Logger.log("Check that the page links correctly navigate to the different sections");
        aboutTheProjectPage
            .jumpToSection("Project details")
            .pageHasMovedToSection("Project details")
            .jumpToSection("Project assignment")
            .pageHasMovedToSection("Project assignment")
            .jumpToSection("Reasons for the conversion")
            .pageHasMovedToSection("Reasons for the conversion")
            .jumpToSection("Advisory board details")
            .pageHasMovedToSection("Advisory board details")
            .jumpToSection("Academy details")
            .pageHasMovedToSection("Academy details")
            .jumpToSection("Incoming trust details")
            .pageHasMovedToSection("Incoming trust details");
    });

    it("Should display 'Not assigned to project' banner and cannot change the project details when viewing a project that is not assigned to the user", () => {
        Logger.log("Go to unassigned project");
        cy.visit(`projects/${teammatesProjectId}/tasks`);

        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Check that the 'Not assigned to project' banner is displayed");
        projectDetailsPage.containsImportantBannerWithMessage(
            "Not assigned to project",
            "This project is not assigned to you and cannot be changed, you can add notes or contacts if required.",
        );

        Logger.log("Check that the user cannot change the project details");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");
        aboutTheProjectPage.linkDoesNotExist("Change");

        cy.visit(`projects/conversions/${teammatesProjectId}/edit`).notAuthorisedToPerformAction();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
