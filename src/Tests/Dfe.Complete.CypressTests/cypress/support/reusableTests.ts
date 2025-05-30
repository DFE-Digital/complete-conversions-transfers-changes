import navBar from "../pages/navBar";
import allProjects from "../pages/projects/allProjects";
import yourTeamProjects from "../pages/projects/yourTeamProjects";
import assignProject from "../pages/projects/assignProject";
import { cypressUser } from "../constants/cypressConstants";
import yourProjects from "../pages/projects/yourProjects";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { currentMonthLong, currentMonthShort } from "cypress/constants/stringTestConstants";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import yourTeamProjectsTable from "cypress/pages/projects/tables/yourTeamProjectsTable";
import { Logger } from "cypress/common/logger";

export function shouldNotHaveAccessToViewHandedOverProjects() {
    cy.visit("/projects/all/in-progress/all");
    allProjects.unableToViewFilter("Handover");
    // cy.visit("/projects/all/handover").notAuthorisedToPerformAction(); // not implemented 187511
}

export function shouldNotHaveAccessToViewYourTeamUnassignedProjects() {
    cy.visit("/projects/team/in-progress");
    yourTeamProjects.unableToViewFilter("Unassigned");
    cy.visit("/projects/team/unassigned").notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewYourProjectsSections() {
    cy.visit("/projects/yours/in-progress").notAuthorisedToPerformAction();
    cy.visit("/projects/yours/added-by").notAuthorisedToPerformAction();
    cy.visit("/projects/yours/completed").notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewYourTeamProjectsSections() {
    cy.visit("/projects/team/unassigned").notAuthorisedToPerformAction();
    cy.visit("/projects/team/in-progress").notAuthorisedToPerformAction();
    cy.visit("/projects/team/new").notAuthorisedToPerformAction();
    cy.visit("/projects/team/users").notAuthorisedToPerformAction();
    cy.visit("/projects/team/completed").notAuthorisedToPerformAction();
    cy.visit("/projects/team/handed-over").notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewProjectExports() {
    navBar.goToAllProjects();
    allProjects.unableToViewFilter("Exports");
    // cy.visit("/projects/all/export").notAuthorisedToPerformAction(); // not implemented
}

export function shouldNotBeAbleToCreateAProject() {
    cy.visit("/projects/yours/in-progress").notAuthorisedToPerformAction();
    cy.visit("/projects/yours/added-by").notAuthorisedToPerformAction();
    cy.visit("/projects/new").notAuthorisedToPerformAction();
    cy.visit("/projects/conversions/new").notAuthorisedToPerformAction();
    cy.visit("/projects/transfers/new").notAuthorisedToPerformAction();
    cy.visit("/projects/conversions/new_mat").notAuthorisedToPerformAction();
    cy.visit("/projects/transfers/new_mat").notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewAndEditUsers() {
    cy.visit("/service-support/users").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToBeAssignedAProject() {
    // not implemented 187369
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
    projectTable.hasTableHeaders([
        "School or academy",
        "URN",
        "Conversion or transfer date",
        "Project type",
        "Region",
        "Assigned project",
    ]);
    yourTeamProjectsTable.assignProject(unassignedProjectSchoolName);
    assignProject.assignTo(cypressUser.username);
    navBar.goToYourProjects();
    yourProjects.goToNextPageUntilFieldIsVisible(unassignedProjectSchoolName);
}

export function shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection() {
    // not implemented
}

export function checkAccessibilityAcrossPages() {
    const visitedUrls = Cypress.env("visitedUrls");
    visitedUrls.forEach((url: string) => {
        cy.visit(url);
        Logger.log("Executing accessibility check for URL: " + url);
        cy.executeAccessibilityTests();
    });
}
