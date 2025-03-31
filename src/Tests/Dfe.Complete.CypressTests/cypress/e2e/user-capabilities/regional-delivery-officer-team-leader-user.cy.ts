import {
    shouldBeAbleToAssignUnassignedProjectsToUsers,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
} from "../../support/reusableTests";
import { before, beforeEach } from "mocha";
import projectRemover from "../../api/projectRemover";
import projectApi from "../../api/projectApi";
import { ProjectBuilder } from "../../api/projectBuilder";
import { nextMonth } from "../../constants/stringTestConstants";

const unassignedProject = ProjectBuilder.createConversionProjectRequest(nextMonth, 103845, "");
const unassignedProjectSchoolName = "Jesson's CofE Primary School (VA)";

describe("Capabilities and permissions of the regional casework services team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${unassignedProject.urn.value}`);
        projectApi.createProject(unassignedProject, "");
    });

    beforeEach(() => {
        cy.login({ role: "RegionalCaseworkServicesTeamLeader" });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should NOT be able to create a project", () => {
        cy.pause();
        shouldNotBeAbleToCreateAProject();
    });

    it.skip("Should NOT be able to be assigned a project", () => {
        shouldNotBeAbleToBeAssignedAProject();
    });

    it("Should be able to assign unassigned projects to users", () => {
        shouldBeAbleToAssignUnassignedProjectsToUsers(unassignedProjectSchoolName);
    });
});
