import navBar from "../../pages/navBar";
import allProjects from "../../pages/projects/allProjects";
import { projectTable } from "../../pages/projects/tables/projectTable";
import {before, beforeEach} from "mocha";
import projectApi from "../../api/projectApi";
import {ProjectBuilder} from "../../api/projectBuilder";
import projectRemover from "../../api/projectRemover";
import dateRangeFilter from "../../pages/projects/dateRangeFilter";

const project = ProjectBuilder.createConversionProjectRequest();
const schoolName = "St Chad's Catholic Primary School";
describe("Data consumer user - view all projects", () => {
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
        const currentMonthString = `${today.toLocaleString('default', { month: 'long' })} ${today.getFullYear()}`;

        navBar.goToAllProjects();
        allProjects
            .filterProjects("By month")
            .containsHeading(`${currentMonthString} to ${currentMonthString}`)
        dateRangeFilter
            .selectDateFrom("May 2025")
            .selectDateTo("Aug 2025")
            .applyDateFilter();
        allProjects.containsHeading("May 2025 to August 2025");
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