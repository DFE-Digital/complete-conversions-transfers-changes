import {
    shouldBeAbleToAssignUnassignedProjectsToUsers,
    shouldNotHaveAccessToViewAndEditUsers,
} from "cypress/support/reusableTests";
import { before, beforeEach } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { nextMonth } from "cypress/constants/stringTestConstants";
import { rdoTeamLeaderUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";

const unassignedProject = ProjectBuilder.createConversionProjectRequest(nextMonth, 103845, "");
const unassignedProjectSchoolName = "Jesson's CofE Primary School (VA)";

describe("Capabilities and permissions of the regional delivery officer team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${unassignedProject.urn.value}`);
        projectApi.createConversionProject(unassignedProject, "");
    });

    beforeEach(() => {
        cy.login({ activeDirectoryId: rdoTeamLeaderUser.adId });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all unassigned team projects after login", () => {
        cy.url().should("include", "/projects/team/unassigned");
    });

    it("Should be able to view 'Your projects', 'Your team projects', 'All projects' and 'Groups' sections", () => {
        navBar.ableToView(["Your projects", "Your team projects", "All projects", "Groups"]);
    });

    it("Should NOT be able to view 'Service support' section", () => {
        navBar.unableToView(["Service support"]);
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
