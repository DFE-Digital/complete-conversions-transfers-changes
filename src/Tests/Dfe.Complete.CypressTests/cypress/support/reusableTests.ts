import homePage from "../pages/homePage";
import navBar from "../pages/navBar";
import allProjects from "../pages/projects/allProjects";
import { projectTable } from "../pages/projects/tables/projectTable";
import { nextMonth } from "../constants/stringTestConstants";
import { CreateProjectRequest } from "../api/apiDomain";
import yourTeamProjects from "../pages/projects/yourTeamProjects";
import yourTeamProjectsRCSViewTable from "../pages/projects/tables/yourTeamProjectsRCSViewTable";
import assignProject from "../pages/projects/assignProject";
import { Username } from "../constants/cypressConstants";
import yourProjects from "../pages/projects/yourProjects";

export function shouldNotBeAbleToViewYourProjects() {
    navBar.unableToViewYourProjects();
    cy.visit("/projects/yours/in-progress").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToViewYourTeamProjects() {
    navBar.unableToViewYourTeamProjects();
    cy.visit("/projects/team/in-progress").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToViewHandedOverProjects() {
    navBar.goToAllProjects();
    allProjects.doesNotContainFilter("Handover");
    cy.visit("/projects/all/handover").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToCreateAProject() {
    homePage.unableToAddAProject();
    cy.visit("/projects/new").notAuthorisedToPerformAction();
    cy.visit("/projects/conversions/new").notAuthorisedToPerformAction();
    cy.visit("/projects/transfer-projects/new").notAuthorisedToPerformAction();
    cy.visit("/projects/conversions/new_mat").notAuthorisedToPerformAction();
    cy.visit("/projects/transfer/new_mat").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToViewAndEditUsers() {
    navBar.unableToViewServiceSupport();
    cy.visit("/service-support/users").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToViewAndEditLocalAuthorities() {
    navBar.unableToViewServiceSupport();
    cy.visit("/service-support/local-authorities").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToViewConversionURNs() {
    navBar.unableToViewServiceSupport();
    cy.visit("/projects/service-support/without-academy-urn").notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToBeAssignedAProject() {
    // not implemented
}

export function shouldOnlyBeAbleToViewNextMonthOfProjects(schoolName: string, project: CreateProjectRequest) {
    // not implemented 187137
    const nextMonthString = `${nextMonth.toLocaleString("default", { month: "short" })} ${nextMonth.getFullYear()}`;
    navBar.goToAllProjects();
    allProjects.filterProjects("By month").containsHeading(nextMonthString);
    projectTable
        .hasTableHeader("School and URN")
        .hasTableHeader("Region")
        .hasTableHeader("Incoming trust")
        .hasTableHeader("All conditions met")
        .hasTableHeader("Confirmed date (Original date)")
        .contains(`${schoolName} ${project.urn.value}`)
        .goTo(`${schoolName} ${project.urn.value}`);
    // projectDetailsPage.containsHeading(schoolName); // not implemented
}

export function shouldBeAbleToViewMultipleMonthsOfProjects() {
    const today = new Date();
    const currentMonthString = `${today.toLocaleString("default", { month: "long" })} ${today.getFullYear()}`;
    navBar.goToAllProjects();
    allProjects.filterProjects("By month").containsHeading(`${currentMonthString} to ${currentMonthString}`);
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
    assignProject.assignTo(Username);
    navBar.goToYourProjects();
    yourProjects.goToNextPageUntilFieldIsVisible(unassignedProjectSchoolName);
}

export function shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection() {
    // not implemented
}
