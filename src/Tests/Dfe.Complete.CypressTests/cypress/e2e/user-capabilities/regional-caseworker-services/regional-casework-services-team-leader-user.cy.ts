import {
    shouldBeAbleToAssignUnassignedProjectsToUsers,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldOnlyBeAbleToViewNextMonthOfProjects,
} from "cypress/support/reusableTests";
import { nextMonth } from "cypress/constants/stringTestConstants";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { before, beforeEach } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { regionalCaseworkerTeamLeaderUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";

const project = ProjectBuilder.createConversionProjectRequest(nextMonth);
const schoolName = "St Chad's Catholic Primary School";
const unassignedProject = ProjectBuilder.createConversionProjectRequest(nextMonth, 103845, "");
const unassignedProjectSchoolName = "Jesson's CofE Primary School (VA)";
describe("Capabilities and permissions of the regional casework services team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${unassignedProject.urn.value}`);
        projectApi.createConversionProject(project);
        projectApi.createConversionProject(unassignedProject, "");
    });

    beforeEach(() => {
        cy.login({ activeDirectoryId: regionalCaseworkerTeamLeaderUser.adId });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all unassigned team projects after login", () => {
        cy.url().should("include", "/projects/team/unassigned");
    });

    it("Should be able to view 'Your team projects', 'All projects' and 'Groups' sections", () => {
        navBar.ableToView(["Your team projects", "All projects", "Groups"]);
    });

    it("Should NOT be able to view 'Your projects' and 'Service support' sections", () => {
        navBar.unableToView(["Your projects", "Service support"]);
    });

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT be able to view Your team projects -> handed over projects", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects.doesNotContainFilter("Handed over");
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });

    it.skip("Should be able to view all Conversions projects by month - next month only", () => {
        shouldOnlyBeAbleToViewNextMonthOfProjects(schoolName, project);
    });

    it.skip("Should be able to assign unassigned projects to users", () => {
        shouldBeAbleToAssignUnassignedProjectsToUsers(unassignedProjectSchoolName);
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
