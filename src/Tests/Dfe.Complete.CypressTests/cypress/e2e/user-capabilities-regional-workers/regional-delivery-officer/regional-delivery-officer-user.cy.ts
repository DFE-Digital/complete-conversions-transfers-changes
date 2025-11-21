import {
    shouldNotHaveAccessToViewAddEditUsers,
    shouldNotHaveAccessToViewConversionURNsPage,
    shouldNotHaveAccessToViewLocalAuthorities,
    shouldNotHaveAccessToViewProjectReports,
    shouldNotHaveAccessToViewYourTeamUnassignedProjects,
} from "cypress/support/reusableTests";
import { beforeEach } from "mocha";
import { cypressUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import yourProjects from "cypress/pages/projects/yourProjects";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import allProjects from "cypress/pages/projects/allProjects";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.regionalWorker.ark,
    provisionalConversionDate: "2027-04-01",
});
let projectId: string;
describe("Capabilities and permissions of the regional delivery officer user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectApi.createAndUpdateConversionProject(project).then((response) => (projectId = response.value));
    });
    beforeEach(() => {
        cy.login(cypressUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user your projects in progress after login", () => {
        cy.url().should("include", "/projects/yours/in-progress");
    });

    it("Should be able to view 'Your projects', 'Your team projects', 'All projects' and 'Groups' sections and filters", () => {
        navBar.ableToView(["Your projects", "Your team projects", "All projects", "Groups"]);
        navBar.goToYourProjects();
        yourProjects.ableToViewFilters(["In progress", "Added by you", "Completed"]);
        navBar.goToYourTeamProjects();
        yourTeamProjects.ableToViewFilters(["In progress", "New", "By user", "Handed over", "Completed"]);
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
        ]);
    });

    it("Should NOT be able to view 'Service support' section", () => {
        navBar.unableToView(["Service support"]);
    });

    it("Should NOT have access to view Your team projects -> unassigned projects", () => {
        shouldNotHaveAccessToViewYourTeamUnassignedProjects();
    });

    it("Should NOT have access to view All projects -> Reports", () => {
        shouldNotHaveAccessToViewProjectReports();
    });

    it("Should NOT have access to view, add or edit users", () => {
        shouldNotHaveAccessToViewAddEditUsers();
    });

    it("Should NOT be able to view and edit local authorities", () => {
        shouldNotHaveAccessToViewLocalAuthorities();
    });

    it("Should NOT be able to view conversion URNs", () => {
        shouldNotHaveAccessToViewConversionURNsPage(projectId);
    });
});
