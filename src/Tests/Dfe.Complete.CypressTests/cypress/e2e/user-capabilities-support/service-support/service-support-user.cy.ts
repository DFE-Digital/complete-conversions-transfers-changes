import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldBeAbleToViewReportsLandingPage,
    shouldNotBeAbleToAddAProjectNote,
    shouldNotBeAbleToAddAProjectTaskNote,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewYourProjectsSections,
    shouldNotHaveAccessToViewYourTeamProjectsSections,
} from "cypress/support/reusableTests";
import navBar from "cypress/pages/navBar";
import { serviceSupportUser } from "cypress/constants/cypressConstants";
import allProjects from "cypress/pages/projects/allProjects";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";

const project = ProjectBuilder.createConversionProjectRequest({ urn: { value: urnPool.support.whitcliffe } });
let projectId: string;
const projectToDelete = ProjectBuilder.createConversionProjectRequest({ urn: { value: urnPool.support.kinnerley } });
let projectToDeleteId: string;
const schoolToDeleteName = "Kinnerley Church of England Controlled Primary School";
describe("Capabilities and permissions of the service support user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(projectToDelete.urn.value);
        projectApi.createConversionProject(project).then((response) => (projectId = response.value));
        projectApi.createConversionProject(projectToDelete).then((response) => (projectToDeleteId = response.value));
    });
    beforeEach(() => {
        cy.login(serviceSupportUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all Service support -> without academy URNs after login", () => {
        cy.url().should("include", "/projects/service-support/without-academy-urn");
    });

    it("Should be able to view 'All projects', 'Groups' and 'Service support' sections and filters", () => {
        navBar.ableToView(["All projects", "Groups", "Service support"]);
        navBar.goToAllProjects();
        allProjects.ableToViewFilters([
            "Handover",
            "In progress",
            "By month",
            "By region",
            "By user",
            "By trust",
            "By local authority",
            "Completed",
            "Statistics",
            "Reports",
        ]);
    });

    it("Should NOT be able to view 'Your projects', 'Your team projects' sections", () => {
        navBar.unableToView(["Your projects", "Your team projects"]);
    });

    it("Should NOT have access to view Your projects sections", () => {
        shouldNotHaveAccessToViewYourProjectsSections();
    });

    it("Should NOT have access to view Your team projects sections", () => {
        shouldNotHaveAccessToViewYourTeamProjectsSections();
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it("Should be able to view the reports landing page", () => {
        shouldBeAbleToViewReportsLandingPage();
    });

    it.skip("Should be able to review projects newly handed over from prepare", () => {
        // not implemented 187511
    });

    it("Should be able to soft delete a project", () => {
        cy.visit(`/projects/${projectToDeleteId}`);
        taskListPage
            .clickButton("Delete project")
            .containsHeading(`Delete ${schoolToDeleteName}`)
            .clickButton("Delete project")
            .containsSuccessBannerWithMessage("The project was deleted.");
        cy.url().should("include", "/projects/all/in-progress/all");
    });

    it("Should NOT be able to create a project", () => {
        shouldNotBeAbleToCreateAProject();
    });

    it("Should NOT be able to add a note to a project", () => {
        shouldNotBeAbleToAddAProjectNote(projectId);
    });

    it("Should NOT be able to add a task note to a project", () => {
        shouldNotBeAbleToAddAProjectTaskNote(projectId);
    });
});
