import {
    shouldBeAbleToChangeTheAddedByUserOfAProject,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldBeAbleToViewReportsLandingPage,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewConversionURNsPage,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldNotHaveAccessToViewLocalAuthorities,
} from "cypress/support/reusableTests";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { before, beforeEach } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import {
    cypressUser,
    regionalCaseworkerTeamLeaderUser,
    regionalCaseworkerUser,
} from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import allProjects from "cypress/pages/projects/allProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import yourTeamProjectsTable from "cypress/pages/projects/tables/yourTeamProjectsTable";
import editUserPage from "cypress/pages/projects/editUserPage";
import { urnPool } from "cypress/constants/testUrns";

const unassignedProject = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.regionalCaseworker.mountjoy },
    handingOverToRegionalCaseworkService: true,
});
const unassignedProjectSchoolName = "Mountjoy House School";
const project = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: urnPool.regionalCaseworker.morda },
});
let projectId: string;
describe("Capabilities and permissions of the regional casework services team leader user", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(unassignedProject.urn.value);
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectApi.createTransferProject(unassignedProject);
        projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
    });

    beforeEach(() => {
        cy.login(regionalCaseworkerTeamLeaderUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all unassigned team projects after login", () => {
        cy.url().should("include", "/projects/team/unassigned");
    });

    it("Should be able to view 'Your team projects', 'All projects' and 'Groups' sections and filters", () => {
        navBar.ableToView(["Your team projects", "All projects", "Groups"]);
        navBar.goToYourTeamProjects();
        yourTeamProjects.ableToViewFilters(["Unassigned", "In progress", "New", "By user", "Completed"]);
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

    it("Should NOT be able to view 'Your projects' and 'Service support' sections", () => {
        navBar.unableToView(["Your projects", "Service support"]);
    });

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT be able to view Your team projects -> handed over projects", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects.unableToViewFilter("Handed over");
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
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
            "Region",
            "Assign project",
        ]);
        yourTeamProjectsTable.assignProject(unassignedProjectSchoolName);
        editUserPage
            .hasLabel("Assign to")
            .assignTo(regionalCaseworkerUser.username)
            .clickButton("Continue")
            .containsSuccessBannerWithMessage("Project has been assigned successfully");
        navBar.goToYourTeamProjects();
        yourTeamProjects.goToLastPage().goToPreviousPageUntilFieldIsVisible(unassignedProjectSchoolName);
        yourTeamProjectsTable
            .withSchool(unassignedProjectSchoolName)
            .columnHasValue("Assigned to", regionalCaseworkerUser.username);
    });

    it("Should be able to change the added by user of the project in internal projects", () => {
        shouldBeAbleToChangeTheAddedByUserOfAProject(project.urn.value, projectId, cypressUser, regionalCaseworkerUser);
    });

    it("Should be able to view the reports landing page", () => {
        shouldBeAbleToViewReportsLandingPage();
    });

    it("Should NOT be able to create a project", () => {
        shouldNotBeAbleToCreateAProject();
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
