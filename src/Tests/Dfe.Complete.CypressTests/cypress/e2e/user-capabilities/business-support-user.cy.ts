import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldNotHaveAccessToViewYourTeamUnassignedProjects,
} from "cypress/support/reusableTests";
import { businessSupportUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import projectRemover from "cypress/api/projectRemover";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import allProjects from "cypress/pages/projects/allProjects";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { currentMonthShort } from "cypress/constants/stringTestConstants";

const date = new Date(2027, 4, 1);
const project = ProjectBuilder.createConversionProjectRequest(date);
const schoolName = "St Chad's Catholic Primary School";
describe("Capabilities and permissions of the business support user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createConversionProject(project);
    });
    beforeEach(() => {
        cy.login(businessSupportUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all projects in progress after login", () => {
        cy.url().should("include", "/projects/all/in-progress/all");
    });

    it("Should only be able to view All projects section", () => {
        navBar.unableToViewTheNavBar();
    });

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT have access to view Your team projects -> unassigned projects", () => {
        shouldNotHaveAccessToViewYourTeamUnassignedProjects();
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        cy.visit("/projects/all/in-progress/all");
        allProjects.filterProjects("By month");
        // .containsHeading(`${currentMonthLong} to ${currentMonthLong}`);
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
            .columnHasValue("Local authority", "Dudley Metropolitan Borough Council")
            // .columnHasValue("Incoming trust", trust) // bug 208086
            .columnHasValue("All conditions met", "Not yet")
            .columnHasValue("Confirmed date (Original date)", "Apr 2027") //todo check
            .goTo(`${schoolName} ${project.urn.value}`);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it.skip("Should be able to view and download csv reports from the export section", () => {
        // not implemented 187516
        shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection();
    });

    it.skip("Should NOT be able to create a project", () => {
        // not implemented
        shouldNotBeAbleToCreateAProject();
    });

    it.skip("Should NOT be able to be assigned a project", () => {
        // not implemented
        shouldNotBeAbleToBeAssignedAProject();
    });

    it.skip("Should NOT be able to soft delete projects", () => {
        // not implemented
    });

    it.skip("Should NOT be able to view and edit local authorities", () => {
        // this can be viewed in the Ruby app currently?
    });

    it.skip("Should NOT be able to view conversion URNs", () => {
        // this can be viewed in the Ruby app currently?
    });
});
