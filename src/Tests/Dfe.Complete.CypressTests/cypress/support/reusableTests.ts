import homePage from "../pages/homePage";
import navBar from "../pages/navBar";
import allProjects from "../pages/projects/allProjects";
import yourTeamProjects from "../pages/projects/yourTeamProjects";
import yourTeamProjectsRCSViewTable from "../pages/projects/tables/yourTeamProjectsRCSViewTable";
import assignProject from "../pages/projects/assignProject";
import { cypressUser } from "../constants/cypressConstants";
import yourProjects from "../pages/projects/yourProjects";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { currentMonthLong, currentMonthShort } from "cypress/constants/stringTestConstants";

export function shouldNotHaveAccessToViewHandedOverProjects() {
    cy.visit("/projects/all/in-progress/all");
    allProjects.doesNotContainFilter("Handover");
    // cy.visit("/projects/all/handover").notAuthorisedToPerformAction(); // not implemented auth
}

export function shouldNotHaveAccessToViewYourTeamUnassignedProjects() {
    cy.visit("/projects/team/in-progress");
    yourTeamProjects.doesNotContainFilter("Unassigned");
    // cy.visit("/projects/team/unassigned").notAuthorisedToPerformAction(); // not implemented auth
}

export function shouldNotHaveAccessToViewProjectExports() {
    navBar.goToAllProjects();
    allProjects.doesNotContainFilter("Exports");
    // cy.visit("/projects/all/export").notAuthorisedToPerformAction(); // not implemented auth
}

export function shouldNotBeAbleToCreateAProject() {
    cy.visit("/projects/yours/in-progress");
    homePage.unableToAddAProject();
    cy.visit("/projects/new").notAuthorisedToPerformAction();
    cy.visit("/projects/conversions/new").notAuthorisedToPerformAction();
    cy.visit("/projects/transfer-projects/new").notAuthorisedToPerformAction();
    cy.visit("/projects/conversions/new_mat").notAuthorisedToPerformAction();
    cy.visit("/projects/transfer/new_mat").notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewAndEditUsers() {
    cy.visit("/service-support/users").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToBeAssignedAProject() {
    // not implemented
}

export function shouldBeAbleToViewMultipleMonthsOfProjects() {
    cy.visit("/projects/all/in-progress/all");
    allProjects.filterProjects("By month").containsHeading(`${currentMonthLong} to ${currentMonthLong}`);
    projectsByMonthPage.filterIsFromDateToDate(currentMonthShort, currentMonthShort);
}

export function shouldBeAbleToAssignUnassignedProjectsToUsers(unassignedProjectSchoolName: string) {
    navBar.goToYourTeamProjects();
    yourTeamProjects
        .filterProjects("Unassigned")
        .containsHeading("Your team unassigned projects")
        .goToNextPageUntilFieldIsVisible(unassignedProjectSchoolName);
    yourTeamProjectsRCSViewTable
        .hasTableHeader("School or academy")
        .hasTableHeader("URN")
        .hasTableHeader("Conversion or transfer date")
        .hasTableHeader("Project type")
        .hasTableHeader("Region")
        .hasTableHeader("Assigned project")
        .assignProject(unassignedProjectSchoolName);
    assignProject.assignTo(cypressUser.username);
    navBar.goToYourProjects();
    yourProjects.goToNextPageUntilFieldIsVisible(unassignedProjectSchoolName);
}

export function shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection() {
    // not implemented
}
