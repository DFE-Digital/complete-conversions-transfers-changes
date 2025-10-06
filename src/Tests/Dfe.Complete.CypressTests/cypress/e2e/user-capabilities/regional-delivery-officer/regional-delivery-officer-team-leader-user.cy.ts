import {
    shouldBeAbleToChangeTheAddedByUserOfAProject,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldBeAbleToViewReportsLandingPage,
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewConversionURNsPage,
    shouldNotHaveAccessToViewLocalAuthorities,
} from "cypress/support/reusableTests";
import { before, beforeEach } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { cypressUser, rdoTeamLeaderUser, regionalCaseworkerUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import yourProjects from "cypress/pages/projects/yourProjects";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import allProjects from "cypress/pages/projects/allProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import yourTeamProjectsTable from "cypress/pages/projects/tables/yourTeamProjectsTable";
import editUserPage from "cypress/pages/projects/editUserPage";
import { urnPool } from "cypress/constants/testUrns";

const unassignedProject = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.regionalDeliveryOfficer.ark },
    handingOverToRegionalCaseworkService: true,
    userAdId: rdoTeamLeaderUser.adId,
});
const unassignedProjectSchoolName = "Ark Globe Academy";
const project = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: urnPool.regionalDeliveryOfficer.longnor },
});
let projectId: string;

describe("Capabilities and permissions of the regional delivery officer team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(unassignedProject.urn.value);
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectApi.createTransferProject(unassignedProject, rdoTeamLeaderUser.email);
        projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
    });

    beforeEach(() => {
        cy.login(rdoTeamLeaderUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all unassigned team projects after login", () => {
        cy.url().should("include", "/projects/team/unassigned");
    });

    it("Should be able to view 'Your projects', 'Your team projects', 'All projects' and 'Groups' sections and filters", () => {
        navBar.ableToView(["Your projects", "Your team projects", "All projects", "Groups"]);
        navBar.goToYourProjects();
        yourProjects.ableToViewFilters(["In progress", "Completed"]);
        navBar.goToYourTeamProjects();
        yourTeamProjects.ableToViewFilters(["Unassigned", "In progress", "New", "By user", "Handed over", "Completed"]);
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
            "Reports",
        ]);
    });

    it("Should NOT be able to view 'Service support' section", () => {
        navBar.unableToView(["Service support"]);
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it("Should be able to assign unassigned projects to users", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects
            .filterProjects("Unassigned")
            .containsHeading("Your team unassigned projects")
            .goToNextPageUntilFieldIsVisible(unassignedProjectSchoolName);
        projectTable.hasTableHeaders([
            "School or academy",
            "URN",
            "Added by",
            "Conversion or transfer date",
            "Project type",
            "Team",
            "Assign project",
        ]);
        yourTeamProjectsTable.assignProject(unassignedProjectSchoolName);
        editUserPage
            .hasLabel("Assign to")
            .assignTo(rdoTeamLeaderUser.username)
            .clickButton("Continue")
            .containsSuccessBannerWithMessage("Project has been assigned successfully");
        navBar.goToYourProjects();
        yourProjects.goToNextPageUntilFieldIsVisible(unassignedProjectSchoolName);
    });

    it("Should be able to change the added by user of the project in internal projects", () => {
        shouldBeAbleToChangeTheAddedByUserOfAProject(project.urn.value, projectId, cypressUser, regionalCaseworkerUser);
    });

    it("Should be able to view the reports landing page", () => {
        shouldBeAbleToViewReportsLandingPage();
    });

    it.skip("Should NOT be able to soft delete projects", () => {
        // not implemented
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });

    it("Should NOT be able to view and edit local authorities", () => {
        shouldNotHaveAccessToViewLocalAuthorities();
    });

    it("Should NOT be able to view conversion URNs", () => {
        shouldNotHaveAccessToViewConversionURNsPage(projectId);
    });
});
