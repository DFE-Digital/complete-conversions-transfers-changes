import {
    shouldBeAbleToAssignUnassignedProjectsToUsers,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewHandedOverProjects,
} from "cypress/support/reusableTests";
import { nextMonth } from "cypress/constants/stringTestConstants";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { before, beforeEach } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { regionalCaseworkerTeamLeaderUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import allProjects from "cypress/pages/projects/allProjects";

const unassignedProject = ProjectBuilder.createConversionProjectRequest(
    nextMonth,
    103846,
    regionalCaseworkerTeamLeaderUser.adId,
);
const unassignedProjectSchoolName = "Cradley CofE Primary School";
describe("Capabilities and permissions of the regional casework services team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${unassignedProject.urn.value}`);
        projectApi.createConversionProject(unassignedProject, regionalCaseworkerTeamLeaderUser.email);
    });

    beforeEach(() => {
        cy.login(regionalCaseworkerTeamLeaderUser);
        cy.acceptCookies();
        cy.visit("/projects/team/in-progress"); // visit '/' goes to unassigned projects that is not implemented yet
    });

    // not implemented 188857
    it.skip("Should direct user to all unassigned team projects after login", () => {
        cy.url().should("include", "/projects/team/unassigned");
    });

    it("Should be able to view 'Your team projects', 'All projects' and 'Groups' sections and filters", () => {
        navBar.ableToView(["Your team projects", "All projects", "Groups"]);
        navBar.goToYourTeamProjects();
        yourTeamProjects.ableToViewFilters([
            // "Unassigned",
            "In progress",
            "New",
            "By user",
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
            "Exports",
        ]);
    });

    it("Should NOT be able to view 'Your projects' and 'Service support' sections", () => {
        navBar.unableToView(["Your projects", "Service support"]);
    });

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT be able to view Your team projects -> handed over projects", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects.unableToViewFilter("Handed over");
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it.skip("Should be able to assign unassigned projects to users", () => {
        shouldBeAbleToAssignUnassignedProjectsToUsers(unassignedProjectSchoolName);
    });

    it("Should NOT be able to create a project", () => {
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
