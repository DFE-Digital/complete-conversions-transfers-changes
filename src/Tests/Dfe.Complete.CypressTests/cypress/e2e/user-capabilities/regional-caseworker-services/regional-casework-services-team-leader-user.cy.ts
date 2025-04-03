import {
    shouldBeAbleToAssignUnassignedProjectsToUsers,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotBeAbleToViewAndEditLocalAuthorities,
    shouldNotBeAbleToViewAndEditUsers,
    shouldNotBeAbleToViewConversionURNs,
    shouldNotBeAbleToViewHandedOverProjects,
    shouldNotBeAbleToViewYourProjects,
    shouldOnlyBeAbleToViewNextMonthOfProjects,
} from "../../../support/reusableTests";
import { nextMonth } from "../../../constants/stringTestConstants";
import { ProjectBuilder } from "../../../api/projectBuilder";
import { before, beforeEach } from "mocha";
import projectRemover from "../../../api/projectRemover";
import projectApi from "../../../api/projectApi";

const project = ProjectBuilder.createConversionProjectRequest(nextMonth);
const schoolName = "St Chad's Catholic Primary School";
const unassignedProject = ProjectBuilder.createConversionProjectRequest(nextMonth, 103845, "");
const unassignedProjectSchoolName = "Jesson's CofE Primary School (VA)";
describe.skip("Capabilities and permissions of the regional casework services team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${unassignedProject.urn.value}`);
        projectApi.createProject(project);
        projectApi.createProject(unassignedProject, "");
    });

    beforeEach(() => {
        cy.login({ role: "RegionalCaseworkServicesTeamLeader" });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should be able to view all Conversions projects by month - next month only", () => {
        shouldOnlyBeAbleToViewNextMonthOfProjects(schoolName, project);
    });

    it("Should be able to assign unassigned projects to users", () => {
        shouldBeAbleToAssignUnassignedProjectsToUsers(unassignedProjectSchoolName);
    });

    it("Should NOT be able to create a project", () => {
        shouldNotBeAbleToCreateAProject();
    });

    it.skip("Should NOT be able to be assigned a project", () => {
        shouldNotBeAbleToBeAssignedAProject();
    });

    it("Should NOT be able view your projects", () => {
        shouldNotBeAbleToViewYourProjects();
    });

    it("Should NOT be able to view handed over projects", () => {
        shouldNotBeAbleToViewHandedOverProjects();
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
