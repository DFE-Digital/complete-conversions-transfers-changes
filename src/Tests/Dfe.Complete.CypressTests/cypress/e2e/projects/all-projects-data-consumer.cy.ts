import navBar from "cypress/pages/navBar";
import allProjects from "cypress/pages/projects/allProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { before, beforeEach } from "mocha";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import dateRangeFilter from "cypress/pages/projects/dateRangeFilter";

const project = ProjectBuilder.createConversionProjectRequest(new Date("2026-06-01"));
const schoolName = "St Chad's Catholic Primary School";
describe.skip("Data consumer user - view all projects", () => {
    // not implemented 187514
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createProject(project);
    });

    beforeEach(() => {
        cy.login({ role: "DataConsumer" });
        cy.acceptCookies();
        cy.visit(`/projects/all/in-progress/all`);
    });

    it("Should be able to view all Conversions projects by month within a date range", () => {
        const today = new Date();
        const currentMonthString = `${today.toLocaleString("default", { month: "long" })} ${today.getFullYear()}`;

        navBar.goToAllProjects();
        allProjects.filterProjects("By month").containsHeading(`${currentMonthString} to ${currentMonthString}`);
        dateRangeFilter.selectDateFrom("May 2026").selectDateTo("Aug 2026").applyDateFilter();
        allProjects.containsHeading("May 2026 to August 2026");
        projectTable
            .hasTableHeader("School and URN")
            .hasTableHeader("Region")
            .hasTableHeader("Local authority")
            .hasTableHeader("Incoming trust")
            .hasTableHeader("All conditions met")
            .hasTableHeader("Confirmed date (Original date)")
            .contains(`${schoolName} ${project.urn.value}`)
            .goTo(`${schoolName} ${project.urn.value}`);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });
});
