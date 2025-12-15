import navBar from "cypress/pages/navBar";
import allProjects from "cypress/pages/projects/allProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { beforeEach } from "mocha";
import {
    currentMonthLong,
    dimensionsTrust,
    macclesfieldTrust,
    nextMonth,
    nextMonthLong,
    nextMonthShort,
} from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import { cypressUser } from "cypress/constants/cypressConstants";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import userProjectTable from "cypress/pages/projects/tables/userProjectTable";
import formAMATProjectTable from "cypress/pages/projects/tables/formAMATProjectTable";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import allProjectsStatisticsPage from "cypress/pages/projects/allProjectsStatisticsPage";
import { getSignificantDateString, significateDateToDisplayDate } from "cypress/support/formatDate";
import { urnPool } from "cypress/constants/testUrns";
import taskHelper from "cypress/api/taskHelper";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.listings.heles,
    provisionalConversionDate: getSignificantDateString(1),
});
let projectId: string;
const schoolName = "Hele's School";
const region = "South West";
const localAuthority = "Plymouth";
const localAuthorityShort = localAuthority.split(" ")[0];
const transferProject = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.listings.queen,
    provisionalTransferDate: getSignificantDateString(1),
});
const transferSchoolName = "Queen Elizabeth Grammar School Penrith";
const transferRegion = "North West";
const transferFormAMatProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.listings.myddle,
    provisionalTransferDate: getSignificantDateString(1),
});
const transferFormAMatSchoolName = "Myddle CofE Primary School";
const prepareProject = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.listings.themount,
    provisionalConversionDate: getSignificantDateString(1),
});
let prepareProjectId: string;
const prepareProjectName = "The Mount School";
const nextMonthShortUS = `${nextMonth.toLocaleString("en-US", { month: "short" })} ${nextMonth.getFullYear()}`; // bug 228624

describe("View all projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(transferProject.urn);
        projectRemover.removeProjectIfItExists(transferFormAMatProject.urn);
        projectRemover.removeProjectIfItExists(prepareProject.urn);
        projectApi.createAndUpdateConversionProject(project).then((response) => {
            projectId = response.value;
            taskHelper.updateExternalStakeholderKickOff(projectId, "completed", getSignificantDateString(1));
        });
        projectApi.createAndUpdateTransferProject(transferProject).then((response) => {
            taskHelper.updateExternalStakeholderKickOff(response.value, "completed", getSignificantDateString(1));
        });
        projectApi.createAndUpdateMatTransferProject(transferFormAMatProject);
        projectApi.createConversionProject(prepareProject).then((response) => (prepareProjectId = response.value));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`/projects/all/in-progress/all`);
    });

    it("Should be able to view my team projects that are handed over", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("Handover")
            .containsHeading("Projects to handover")
            .goToNextPageUntilFieldIsVisible(prepareProjectName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Incoming trust",
                "Provisional conversion or transfer date",
                "Advisory board date",
                "Project type",
                "Add handover details",
            ])
            .withSchool(prepareProjectName)
            .columnHasValue("URN", `${prepareProject.urn}`)
            .columnHasValue("Incoming trust", macclesfieldTrust.name)
            .columnHasValue("Provisional conversion or transfer date", nextMonthShort)
            .columnHasValue("Advisory board date", significateDateToDisplayDate(prepareProject.advisoryBoardDate))
            .columnHasValue("Project type", "Conversion")
            .columnHasValueWithLink(
                "Add handover details",
                "Add handover details",
                `/projects/all/handover/${prepareProjectId}/check`,
            );
    });

    it("Should be able to view newly created conversion project in All projects in progress and Conversions projects", () => {
        navBar.goToAllProjects();
        allProjects
            .containsHeading("All projects in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Conversion or transfer date",
                "Project type",
                "Form a MAT project?",
                "Assigned to",
            ])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username);
        allProjects
            .viewConversionsProjects()
            .containsHeading("All conversions in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion date", "Form a MAT project?", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn}`)
            .columnHasValue("Conversion date", nextMonthShort)
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be able to view newly created transfer project in All projects in progress and Transfers projects", () => {
        navBar.goToAllProjects();
        allProjects
            .containsHeading("All projects in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(transferSchoolName);
        projectTable
            .withSchool(transferSchoolName)
            .columnHasValue("URN", `${transferProject.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Transfer")
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username);
        allProjects
            .viewTransfersProjects()
            .containsHeading("All transfers in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(transferSchoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Transfer date", "Form a MAT project?", "Assigned to"])
            .withSchool(transferSchoolName)
            .columnHasValue("URN", `${transferProject.urn}`)
            .columnHasValue("Transfer date", nextMonthShort)
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(transferSchoolName);
        projectDetailsPage.containsHeading(transferSchoolName);
    });

    it("Should be able to view a form a MAT transfer project in All projects in progress, Transfers projects and Form a MAT projects (by Trust) ", () => {
        navBar.goToAllProjects();
        allProjects
            .containsHeading("All projects in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(transferFormAMatSchoolName);
        projectTable
            .withSchool(transferFormAMatSchoolName)
            .columnHasValue("URN", `${transferFormAMatProject.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Transfer")
            .columnHasValue("Form a MAT project?", "Yes")
            .columnHasValue("Assigned to", cypressUser.username);
        allProjects
            .viewTransfersProjects()
            .containsHeading("All transfers in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(transferFormAMatSchoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Transfer date", "Form a MAT project?", "Assigned to"])
            .withSchool(transferFormAMatSchoolName)
            .columnHasValue("URN", `${transferFormAMatProject.urn}`)
            .columnHasValue("Transfer date", nextMonthShort)
            .columnHasValue("Form a MAT project?", "Yes")
            .columnHasValue("Assigned to", cypressUser.username);
        allProjects
            .viewFormAMatProjects()
            .containsHeading("All form a MAT projects in progress")
            .goToNextPageUntilFieldIsVisible(transferFormAMatSchoolName);
        formAMATProjectTable
            .hasTableHeaders(["Trust", "TRN", "Schools Included"])
            .withTrust(dimensionsTrust.name)
            .columnHasValue("TRN", dimensionsTrust.referenceNumber)
            .columnContainsValue("Schools Included", transferFormAMatSchoolName)
            .goTo(dimensionsTrust.name);
        allProjects.containsHeading(dimensionsTrust.name);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Conversion or transfer date",
                "Project type",
                "Assigned to",
                "State",
            ])
            .withSchool(transferFormAMatSchoolName)
            .columnHasValue("URN", `${transferFormAMatProject.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Transfer")
            .columnHasValue("Assigned to", cypressUser.username)
            .columnHasValue("State", "Active")
            .goTo(transferFormAMatSchoolName);
        projectDetailsPage.containsHeading(transferFormAMatSchoolName);
    });

    it("Should be able to view all Conversions projects by month - next month only", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By month").containsHeading(nextMonthLong);
        projectsByMonthPage.filterDoesNotExist();
        projectTable
            .hasTableHeaders([
                "School and URN",
                "Region",
                "Local authority",
                "Incoming trust",
                "All conditions met",
                "Confirmed date (Original date)",
            ])
            .contains(`${schoolName} ${project.urn}`)
            .withSchool(`${schoolName} ${project.urn}`)
            .columnHasValue("Region", region)
            .columnHasValue("Local authority", localAuthorityShort)
            .columnHasValue("Incoming trust", macclesfieldTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("All conditions met", "Not yet")
            .columnHasValue("Confirmed date (Original date)", `${nextMonthShortUS} (${nextMonthShortUS})`) // bug 228624
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be able to view all Transfer projects by month - next month only", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By month").containsHeading(nextMonthLong);
        projectsByMonthPage.filterDoesNotExist().viewTransferProjects();
        projectTable
            .hasTableHeaders([
                "School and URN",
                "Region",
                "Outgoing trust",
                "Incoming trust",
                "Authority to proceed",
                "Confirmed date (Original date)",
            ])
            .contains(`${transferSchoolName}`)
            .withSchool(`${transferSchoolName}`)
            .columnHasValue("Region", transferRegion)
            .columnHasValue("Outgoing trust", macclesfieldTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("Incoming trust", dimensionsTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("Authority to proceed", "Not yet")
            .columnHasValue("Confirmed date (Original date)", `${nextMonthShortUS} (${nextMonthShortUS})`) // bug 228624
            .goTo(`${transferSchoolName}`);
        projectDetailsPage.containsHeading(transferSchoolName);
    });

    it("Should be able to view all projects by region and all a region's projects", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By region").containsHeading("All projects by region");
        projectTable.hasTableHeaders(["Region", "Conversions", "Transfers"]).contains(region).filterBy(region);
        allProjects.containsHeading(`Projects for ${region}`).goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Conversion")
            .columnContainsValue("Assigned to", cypressUser.username)
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be able to view all projects by user and all a user's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By user")
            .containsHeading("All projects by user")
            .goToNextPageUntilFieldIsVisible(cypressUser.username);
        userProjectTable
            .hasTableHeaders(["User name", "Email", "Team", "Conversions", "Transfers"])
            .withUser(cypressUser.username)
            .columnHasValue("Email", cypressUser.email)
            .columnHasValue("Team", "London")
            .goToUserProjects(cypressUser.username);
        allProjects.containsHeading(`Projects for ${cypressUser.username}`).goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type"])
            .contains(schoolName)
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Conversion")
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be able to view all projects by trust and all a trust's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By trust")
            .containsHeading("All projects by trust")
            .goToNextPageUntilFieldIsVisible(macclesfieldTrust.name);
        projectTable
            .hasTableHeaders(["Trust", "Group identifier", "Conversions", "Transfers"])
            .filterBy(macclesfieldTrust.name);
        allProjects
            .containsHeading(`Projects for ${macclesfieldTrust.name.toUpperCase()}`)
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be able to view all projects by local authority and all a local authority's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By local authority")
            .containsHeading("All projects by local authority")
            .goToNextPageUntilFieldIsVisible(localAuthority);
        projectTable.hasTableHeaders(["Local authority", "Code", "Conversions", "Transfers"]).filterBy(localAuthority);
        allProjects.containsHeading(`Projects for ${localAuthority}`);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn}`)
            .columnHasValue("Conversion or transfer date", nextMonthShort)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be able to view all completed projects", () => {
        projectApi.completeProject(projectId);

        navBar.goToAllProjects();
        allProjects
            .filterProjects("Completed")
            .containsHeading("All completed projects")
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Team",
                "Type of project",
                "Conversion or transfer date",
                "Project completion date",
            ])
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be able to view all projects statistics and jump to sections", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("Statistics").containsHeading("Statistics");

        allProjectsStatisticsPage
            .jumpToSection("Overview of all projects")
            .pageHasMovedToSection("Overview of all projects")
            .tableHasTableHeaders("Overview of all projects", ["Detail", "Conversions", "Transfers"])
            .jumpToSection("Projects with Regional casework services")
            .pageHasMovedToSection("Projects with Regional casework services")
            .tableHasTableHeaders("Projects with Regional casework services", ["Detail", "Conversions", "Transfers"])
            .jumpToSection("Projects not with Regional casework services")
            .pageHasMovedToSection("Projects not with Regional casework services")
            .tableHasTableHeaders("Projects not with Regional casework services", [
                "Detail",
                "Conversions",
                "Transfers",
            ])
            .jumpToSection("Conversion projects per region")
            .pageHasMovedToSection("Conversion projects per region")
            .tableHasTableHeaders("Conversion projects per region", [
                "Region",
                "In-progress projects",
                "Completed projects",
                "Unassigned projects",
                "All projects",
            ])
            .jumpToSection("Transfer projects per region")
            .pageHasMovedToSection("Transfer projects per region")
            .tableHasTableHeaders("Transfer projects per region", [
                "Region",
                "In-progress projects",
                "Completed projects",
                "Unassigned projects",
                "All projects",
            ])
            .jumpToSection("6 month view of all project openers")
            .pageHasMovedToSection("6 month view of all project openers")
            .tableHasTableHeaders("6 month view of all project openers", [
                "Project opener date",
                "Conversions",
                "Transfers",
            ])
            .jumpToSection("New projects this month")
            .pageHasMovedToSection(`New projects this month (${currentMonthLong})`)
            .jumpToSection("Users per team")
            .pageHasMovedToSection("Users per team")
            .tableHasTableHeaders("Users per team", ["Team", "Number of users"]);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
