import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { giasUrl, groupReferenceNumber, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { significateDateToDisplayDate } from "cypress/support/formatDate";

const project = ProjectBuilder.createConversionProjectRequest();
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
describe("About the project page - conversion projects: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${projectFormAMAT.urn.value}`);
        projectRemover.removeProjectIfItExists(`${teammatesProject.urn.value}`);
        projectApi.createConversionProject(project).then((response) => {
            projectId = response.value;
            projectApi.updateProjectAcademyUrn(projectId, academy.urn);
        });
        projectApi.createMatConversionProject(projectFormAMAT).then((response) => (projectFormAMATId = response.value));
        projectApi
            .createMatConversionProject(teammatesProject, rdoLondonUser.email)
            .then((response) => (teammatesProjectId = response.value));
        changeLinkPath = `/projects/conversions/${projectId}/edit#`;
        formAMATChangeLinkPath = `/projects/conversions/${projectFormAMATId}/edit#`;
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
            .hasIncomingTrust(macclesfieldTrust.name)
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
            .hasValue(groupReferenceNumber)
            // .hasChangeLink(`${changeLinkPath}group-reference-number`) // not implemented 219174

            .subSection("Project assignment")
            .hasSubHeading("Project assignment")
            .summaryShows("Are you handing this project over to RCS (Regional Casework Services)?")
            .hasValue(project.handingOverToRegionalCaseworkService ? "Yes" : "No")
            // .hasChangeLink(`${changeLinkPath}project-assignment`) // not implemented 219174

            .subSection("Reasons for the conversion")
            .hasSubHeading("Reasons for the conversion")
            .summaryShows("Has a directive academy order been issued?")
            .hasValue(project.hasAcademyOrderBeenIssued ? "Yes" : "No")
            // .hasChangeLink(`${changeLinkPath}directive-academy-order`) // not implemented 219174
            .summaryShows("Is this conversion due to intervention following 2RI?")
            .hasValue(project.isDueTo2Ri ? "Yes" : "No")
            // .hasChangeLink(`${changeLinkPath}two-requires-improvement`) // not implemented 219174

            .subSection("Advisory board details")
            .hasSubHeading("Advisory board details")
            .summaryShows("Date of advisory board")
            .hasValue(significateDateToDisplayDate(project.advisoryBoardDate))
            // .hasChangeLink(`${changeLinkPath}advisory-board`) // not implemented 219174
            .summaryShows("Conditions from advisory board")
            .hasValue(project.advisoryBoardConditions)
            // .hasChangeLink(`${changeLinkPath}advisory-board`) // not implemented 219174

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
            // .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`) // not implemented 219174

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
                `${macclesfieldTrust.name.toUpperCase()} View the trust information in GIAS (opens in new tab)`,
                `${giasUrl}/Groups/Search?GroupSearchModel.Text=${macclesfieldTrust.ukprn}`,
            )
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue(macclesfieldTrust.ukprn)
            // .hasChangeLink(`${changeLinkPath}incoming-trust-ukprn`) // not implemented 219174
            .summaryShows("Group ID (identifier)")
            .hasValue(macclesfieldTrust.referenceNumber)
            .summaryShows("Companies House number")
            .hasValueWithLink(
                `${macclesfieldTrust.companiesHouseNumber} View the Companies House information (opens in new tab)`,
                `https://find-and-update.company-information.service.gov.uk/company/${macclesfieldTrust.companiesHouseNumber}`,
            )
            .summaryShows("New trust reference number (TRN)")
            .hasValue("")
            .summaryShows("Address")
            .hasValue("Macclesfield College Macclesfield SK11 8LF")
            .summaryShows("SharePoint folder")
            .hasValueWithLink(
                "View the trust SharePoint folder (opens in new tab)",
                project.incomingTrustSharepointLink,
            );
        // .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`); // not implemented 219174
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
            // .hasChangeLink(`${changeLinkPath}group-reference-number`) // not implemented 219174

            .subSection("Project assignment")
            .hasSubHeading("Project assignment")
            .summaryShows("Are you handing this project over to RCS (Regional Casework Services)?")
            .hasValue(projectFormAMAT.handingOverToRegionalCaseworkService ? "Yes" : "No")
            // .hasChangeLink(`${changeLinkPath}project-assignment`) // not implemented 219174

            .subSection("Reasons for the conversion")
            .hasSubHeading("Reasons for the conversion")
            .summaryShows("Has a directive academy order been issued?")
            .hasValue(projectFormAMAT.hasAcademyOrderBeenIssued ? "Yes" : "No")
            // .hasChangeLink(`${changeLinkPath}directive-academy-order`) // not implemented 219174
            .summaryShows("Is this conversion due to intervention following 2RI?")
            .hasValue(projectFormAMAT.isDueTo2Ri ? "Yes" : "No")
            // .hasChangeLink(`${changeLinkPath}two-requires-improvement`) // not implemented 219174

            .subSection("Advisory board details")
            .hasSubHeading("Advisory board details")
            .summaryShows("Date of advisory board")
            .hasValue(significateDateToDisplayDate(projectFormAMAT.advisoryBoardDate))
            // .hasChangeLink(`${changeLinkPath}advisory-board`) // not implemented 219174
            .summaryShows("Conditions from advisory board")
            .hasValue(projectFormAMAT.advisoryBoardConditions)
            // .hasChangeLink(`${changeLinkPath}advisory-board`) // not implemented 219174

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
            // .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`) // not implemented 219174

            .subSection("Academy details")
            .hasSubHeading("Academy details")
            .hasNoAcademyDetailsAsURNNotProvided()

            .subSection("Incoming trust details")
            .hasSubHeading("Incoming trust details")
            .summaryShows("Name")
            .hasValue(macclesfieldTrust.name)
            .summaryShows("UKPRN (UK provider reference number)")
            .hasValue("")
            // .hasChangeLink(`${changeLinkPath}incoming-trust-ukprn`) // not implemented 219174
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
            );
        // .hasChangeLink(`${changeLinkPath}sharepoint-folder-links`); // not implemented 219174
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

    it("Should display 'Not assigned to project' banner when viewing a project that is not assigned to the user", () => {
        Logger.log("Go to unassigned project");
        cy.visit(`projects/${teammatesProjectId}/tasks`);

        Logger.log("Go to the about project section");
        projectDetailsPage.navigateTo("About the project").containsSubHeading("About the project");

        Logger.log("Check that the 'Not assigned to project' banner is displayed");
        projectDetailsPage.containsImportantBannerWithMessage(
            "Not assigned to project",
            "This project is not assigned to you and cannot be changed, you can add notes or contacts if required.",
        );
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
