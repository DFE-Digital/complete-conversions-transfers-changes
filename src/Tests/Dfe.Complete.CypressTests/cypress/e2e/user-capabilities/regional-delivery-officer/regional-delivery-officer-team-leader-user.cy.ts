import {
    shouldBeAbleToAssignUnassignedProjectsToUsers,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldNotHaveAccessToViewAndEditUsers,
} from "cypress/support/reusableTests";
import { before, beforeEach } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { nextMonth } from "cypress/constants/stringTestConstants";
import { rdoTeamLeaderUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import yourProjects from "cypress/pages/projects/yourProjects";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import allProjects from "cypress/pages/projects/allProjects";

const unassignedProject = ProjectBuilder.createConversionProjectRequest(nextMonth, 103845, rdoTeamLeaderUser.adId);
const unassignedProjectSchoolName = "Jesson's CofE Primary School (VA)";

// skipped as visiting / goes to unassigned projects that is not implemented yet
describe.skip("Capabilities and permissions of the regional delivery officer team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${unassignedProject.urn.value}`);
        projectApi.createConversionProject(unassignedProject, rdoTeamLeaderUser.email);
    });

    beforeEach(() => {
        cy.login(rdoTeamLeaderUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all unassigned team projects after login", () => {
        cy.url().should("include", "/projects/team/unassigned");
    });

    it("Should be able to view 'Your projects', 'Your team projects', 'All projects' and 'Groups' sections and filters", () => {
        navBar.ableToView(["Your projects", "Your team projects", "All projects", "Groups"]);
        navBar.goToYourProjects();
        yourProjects.ableToViewFilters(["In progress", "Completed"]);
        navBar.goToYourTeamProjects();
        yourTeamProjects.ableToViewFilters([
            // "Unassigned",
            "In progress",
            "New",
            "By user",
            "Handed over",
            "Completed",
        ]);
        navBar.goToAllProjects();
        allProjects.ableToViewFilters([
            "In progress",
            "By month",
            "By region",
            "By user",
            "By trust",
            "By local authority",
            "Completed",
            "Statistics",
            "Exports"
        ]);
    });

    it("Should NOT be able to view 'Service support' section", () => {
        navBar.unableToView(["Service support"]);
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it.skip("Should be able to assign unassigned projects to users", () => {
        // not implemented
        shouldBeAbleToAssignUnassignedProjectsToUsers(unassignedProjectSchoolName);
    });

    it.skip("Should NOT be able to soft delete projects", () => {
        // not implemented
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });

    it.skip("Should NOT be able to view and edit local authorities", () => {
        // this can be viewed in the Ruby app currently?
    });

    it.skip("Should NOT be able to view conversion URNs", () => {
        // this can be viewed in the Ruby app currently?
    });
});
