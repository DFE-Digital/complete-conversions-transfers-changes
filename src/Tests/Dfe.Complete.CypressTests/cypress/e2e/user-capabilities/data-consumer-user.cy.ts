import { beforeEach } from "mocha";
import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToViewReportsLandingPage,
    shouldNotBeAbleToAddAProjectNote,
    shouldNotBeAbleToAddAProjectTaskNote,
    shouldNotBeAbleToSoftDeleteAProject,
    shouldNotHaveAccessToViewAddEditUsers,
    shouldNotHaveAccessToViewConversionURNsPage,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldNotHaveAccessToViewLocalAuthorities,
    shouldNotHaveAccessToViewYourProjectsSections,
    shouldNotHaveAccessToViewYourTeamProjectsSections,
} from "cypress/support/reusableTests";
import { dataConsumerUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import allProjects from "cypress/pages/projects/allProjects";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import { currentMonthLong, currentMonthShort, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import taskHelper from "cypress/api/taskHelper";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.userCapabilities.longnor,
    provisionalConversionDate: "2027-04-01",
});
let projectId: string;
const schoolName = "Whitcliffe Mount School";
describe("Capabilities and permissions of the data consumer user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectApi.createAndUpdateConversionProject(project).then((response) => {
            projectId = response.value;
            taskHelper.updateExternalStakeholderKickOff(projectId, "completed", "2027-04-01");
        });
    });
    beforeEach(() => {
        cy.login(dataConsumerUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all projects in progress after login", () => {
        cy.url().should("include", "/projects/all/in-progress/all");
    });

    it("Should only be able to view All projects section, with all the expected filters", () => {
        navBar.unableToViewTheNavBar();
        allProjects.ableToViewFilters([
            "In progress",
            "By month",
            "By region",
            "By user",
            "By trust",
            "By local authority",
            "Completed",
            "Statistics",
            "Reports",
        ]);
    });

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT have access to view Your projects sections", () => {
        shouldNotHaveAccessToViewYourProjectsSections();
    });

    it("Should NOT have access to view Your team projects sections", () => {
        shouldNotHaveAccessToViewYourTeamProjectsSections();
    });

    it("Should NOT have access to view, add or edit users", () => {
        shouldNotHaveAccessToViewAddEditUsers();
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        cy.visit("/projects/all/in-progress/all");
        allProjects.filterProjects("By month").containsHeading(`${currentMonthLong} to ${currentMonthLong}`);
        projectsByMonthPage
            .filterIsFromDateToDate(currentMonthShort, currentMonthShort)
            .filterDateRange("Apr 2027", "May 2027");
        projectTable
            .hasTableHeaders([
                "School and URN",
                "Region",
                "Local authority",
                "Incoming trust",
                "All conditions met",
                "Confirmed date (Original date)",
            ])
            .withSchool(`${schoolName} ${project.urn}`)
            .columnHasValue("Region", "Yorkshire and the Humber")
            .columnHasValue("Local authority", "Kirklees")
            .columnHasValue("Incoming trust", macclesfieldTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("All conditions met", "Not yet")
            .columnHasValue("Confirmed date (Original date)", "Apr 2027 (Apr 2027)")
            .goTo(schoolName);
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should show message when 'from' date is after 'to' date and default to current month when viewing projects by month", () => {
        cy.visit("/projects/all/in-progress/all");
        allProjects.filterProjects("By month");

        projectsByMonthPage
            .filterDateRange("May 2025", "Apr 2025")
            .containsImportantBannerWithMessage("The 'from' date cannot be after the 'to' date")
            .filterIsFromDateToDate(currentMonthShort, currentMonthShort);
    });

    it("Should be able to view the reports landing page", () => {
        shouldBeAbleToViewReportsLandingPage();
    });

    it("Should NOT be able to add a note to a project", () => {
        shouldNotBeAbleToAddAProjectNote(projectId);
    });

    it("Should NOT be able to add a task note to a project", () => {
        shouldNotBeAbleToAddAProjectTaskNote(projectId);
    });

    it("Should NOT be able to soft delete projects", () => {
        shouldNotBeAbleToSoftDeleteAProject(projectId);
    });

    it("Should NOT be able to view and edit local authorities", () => {
        shouldNotHaveAccessToViewLocalAuthorities();
    });

    it("Should NOT be able to view conversion URNs", () => {
        shouldNotHaveAccessToViewConversionURNsPage(projectId);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
