import { beforeEach } from "mocha";
import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection,
    shouldNotBeAbleToAddAProjectNote,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewConversionURNsPage,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldNotHaveAccessToViewLocalAuthorities,
    shouldNotHaveAccessToViewYourProjectsSections,
    shouldNotHaveAccessToViewYourTeamProjectsSections,
} from "cypress/support/reusableTests";
import { businessSupportUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import projectRemover from "cypress/api/projectRemover";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import allProjects from "cypress/pages/projects/allProjects";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { currentMonthLong, currentMonthShort, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";

const date = new Date("2027-04-01");
const project = ProjectBuilder.createConversionProjectRequest(date);
let projectId: string;
const schoolName = "St Chad's Catholic Primary School";
describe("Capabilities and permissions of the business support user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createConversionProject(project).then((response) => (projectId = response.value));
    });
    beforeEach(() => {
        cy.login(businessSupportUser);
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
            "Exports",
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

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
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
            .withSchool(`${schoolName} ${project.urn.value}`)
            .columnHasValue("Region", "West Midlands")
            .columnHasValue("Local authority", "Dudley")
            .columnHasValue("Incoming trust", macclesfieldTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("All conditions met", "Not yet")
            .columnHasValue("Confirmed date (Original date)", "Apr 2027")
            .goTo(`${schoolName} ${project.urn.value}`);
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

    it.skip("Should be able to view and download csv reports from the export section", () => {
        // not implemented 187516
        shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection();
    });

    it("Should NOT be able to create a project", () => {
        shouldNotBeAbleToCreateAProject();
    });

    it("Should NOT be able to add a note to a project", () => {
        shouldNotBeAbleToAddAProjectNote(projectId);
    });

    it.skip("Should NOT be able to soft delete projects", () => {
        // not implemented
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
