import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewReportsLandingPage,
    shouldBeAbleToViewMultipleMonthsOfProjects, shouldNotBeAbleToAddAProjectNote,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewConversionURNsPage,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldNotHaveAccessToViewLocalAuthorities,
    shouldNotHaveAccessToViewYourProjectsSections,
    shouldNotHaveAccessToViewYourTeamProjectsSections
} from "cypress/support/reusableTests";
import { dataConsumerUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import allProjects from "cypress/pages/projects/allProjects";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";

const date = new Date("2027-04-01");
const project = ProjectBuilder.createConversionProjectRequest(date);
let projectId: string;
describe("Capabilities and permissions of the data consumer user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createConversionProject(project).then((response) => (projectId = response.value));
    });
    beforeEach(() => {
        cy.login(dataConsumerUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all projects in progress after login", () => {
        cy.url().should("include", "/projects/all/in-progress/all");
    });

    it("Should only be able to view All projects section, with all the expected filters", () => {
        navBar.unableToViewTheNavBar();
        allProjects.ableToViewFilters([
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

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT have access to view Your projects sections", () => {
        shouldNotHaveAccessToViewYourProjectsSections();
    });

    it("Should NOT have access to view Your team projects sections", () => {
        shouldNotHaveAccessToViewYourTeamProjectsSections();
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it("Should be able to view the reports landing page", () => {
        shouldBeAbleToViewReportsLandingPage();
    });

    it("Should NOT be able to create a project", () => {
        shouldNotBeAbleToCreateAProject();
    });

    it("Should NOT be able to add a note to a project", () => {
        shouldNotBeAbleToAddAProjectNote(projectId);
    });

    it.skip("Should NOT be able to soft delete projects", () => {
        // not implemented
    });

    it("Should NOT be able to view and edit local authorities", () => {
        shouldNotHaveAccessToViewLocalAuthorities();
    });

    it("Should NOT be able to view conversion URNs", () => {
        shouldNotHaveAccessToViewConversionURNsPage(projectId);
    });
});
