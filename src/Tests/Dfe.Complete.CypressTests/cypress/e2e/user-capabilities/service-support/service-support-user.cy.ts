import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldBeAbleToViewReportsLandingPage,
    shouldNotBeAbleToAddAProjectNote,
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
import { nextMonth } from "cypress/constants/stringTestConstants";

const project = ProjectBuilder.createConversionProjectRequest(nextMonth);
let projectId: string;
describe("Capabilities and permissions of the service support user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createConversionProject(project).then((response) => (projectId = response.value));
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

    it.skip("Should be able to soft delete a project", () => {
        // not implemented
    });

    it.skip("Should be able to create users", () => {
        // not implemented 187527
    });

    it.skip("Should be able to edit users", () => {
        // not implemented 187527
    });

    it("Should NOT be able to create a project", () => {
        shouldNotBeAbleToCreateAProject();
    });

    it("Should NOT be able to add a note to a project", () => {
        shouldNotBeAbleToAddAProjectNote(projectId);
    });
});
