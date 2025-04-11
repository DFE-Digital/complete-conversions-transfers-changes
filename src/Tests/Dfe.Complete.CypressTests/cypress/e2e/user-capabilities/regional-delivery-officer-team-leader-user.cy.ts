import {
    shouldBeAbleToAssignUnassignedProjectsToUsers,
    shouldNotBeAbleToViewAndEditLocalAuthorities,
    shouldNotBeAbleToViewAndEditUsers,
    shouldNotBeAbleToViewConversionURNs,
} from "cypress/support/reusableTests";
import { before, beforeEach } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { nextMonth } from "cypress/constants/stringTestConstants";
import { rdoTeamLeaderUser } from "cypress/constants/cypressConstants";

const unassignedProject = ProjectBuilder.createConversionProjectRequest(nextMonth, 103845, "");
const unassignedProjectSchoolName = "Jesson's CofE Primary School (VA)";

describe.skip("Capabilities and permissions of the regional casework services team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${unassignedProject.urn.value}`);
        projectApi.createConversionProject(unassignedProject, "");
    });

    beforeEach(() => {
        cy.login({ activeDirectoryId: rdoTeamLeaderUser.adId });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should be able to assign unassigned projects to users", () => {
        shouldBeAbleToAssignUnassignedProjectsToUsers(unassignedProjectSchoolName);
    });

    it.skip("Should NOT be able to soft delete projects", () => {
        // not implemented
    });

    it.skip("Should NOT be able to view and edit users", () => {
        // not implemented
        shouldNotBeAbleToViewAndEditUsers();
    });

    it.skip("Should NOT be able to view and edit local authorities", () => {
        // not implemented
        shouldNotBeAbleToViewAndEditLocalAuthorities();
    });

    it.skip("Should NOT be able to view conversion URNs", () => {
        // not implemented
        shouldNotBeAbleToViewConversionURNs();
    });
});
