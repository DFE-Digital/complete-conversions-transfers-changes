import navBar from "../pages/navBar";
import allProjects from "../pages/projects/allProjects";
import yourTeamProjects from "../pages/projects/yourTeamProjects";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { currentMonthLong, currentMonthShort } from "cypress/constants/stringTestConstants";
import { Logger } from "cypress/common/logger";
import internalContactsPage from "cypress/pages/projects/projectDetails/internalContactsPage";
import { TestUser } from "cypress/constants/TestUser";
import notePage from "cypress/pages/projects/projectDetails/notePage";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";

export function shouldNotHaveAccessToViewHandedOverProjects() {
    cy.visit("/projects/all/in-progress/all");
    allProjects.unableToViewFilter("Handover");
    cy.visit("/projects/all/handover").notAuthorisedToPerformAction();
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

export function shouldNotHaveAccessToViewProjectReports() {
    navBar.goToAllProjects();
    allProjects.unableToViewFilter("Reports");
    cy.visit("/projects/all/reports").notAuthorisedToPerformAction();
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

export function shouldNotBeAbleToSoftDeleteAProject(projectId: string) {
    const pages = ["tasks", "information", "notes", "external-contacts", "internal-contacts", "date-history"];
    for (const page of pages) {
        cy.visit(`/projects/${projectId}/${page}`);
        projectDetailsPage.doesntContain(/Delete project/i);
    }
    cy.visit(`/projects/${projectId}/confirm_delete`).notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToAddAProjectNote(projectId: string) {
    cy.visit(`/projects/${projectId}/notes`);
    notePage.clickButton("Add note").notAuthorisedToPerformThisActionBanner();
    cy.visit(`/projects/${projectId}/notes/new`).notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToAddAProjectTaskNote(projectId: string) {
    cy.visit(`/projects/${projectId}/tasks/handover`);
    notePage.clickButton("Add a new task note").notAuthorisedToPerformThisActionBanner();
    cy.visit(`/projects/${projectId}/notes/new?task_identifier=handover`).notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewAndEditUsers() {
    cy.visit("/service-support/users").notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewLocalAuthorities() {
    cy.visit("/service-support/local-authorities").notAuthorisedToPerformAction();
    cy.visit("/service-support/local-authorities/new").notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewConversionURNsPage(projectId: string) {
    cy.visit("/projects/service-support/without-academy-urn").notAuthorisedToPerformAction();
    cy.visit("/projects/service-support/with-academy-urn").notAuthorisedToPerformAction();
    cy.visit(`/projects/${projectId}/academy-urn`).notAuthorisedToPerformAction();
}

export function shouldBeAbleToViewMultipleMonthsOfProjects() {
    cy.visit("/projects/all/in-progress/all");
    allProjects.filterProjects("By month").containsHeading(`${currentMonthLong} to ${currentMonthLong}`);
    projectsByMonthPage.filterIsFromDateToDate(currentMonthShort, currentMonthShort);
}

export function shouldBeAbleToViewReportsLandingPage() {
    cy.visit("/projects/all/in-progress/all");
    allProjects
        .filterProjects("Reports")
        .containsHeading("Reports")
        .contains("You can now view and download reports on academy conversions and transfers in Power BI.")
        .containsSubHeading("Conversions")
        .containsSubHeading("Transfers")
        .contains("The reports open in a new tab and include guidance to help you find the data you need.")
        .hasButton("View reports in Power BI");
}

export function checkAccessibilityAcrossPages() {
    const visitedUrls = Cypress.env("visitedUrls");
    visitedUrls.forEach((url: string) => {
        cy.visit(url, { failOnStatusCode: false });
        Logger.log("Executing accessibility check for URL: " + url);
        cy.executeAccessibilityTests();
    });
}

export function shouldBeAbleToChangeTheAddedByUserOfAProject(
    projectUrn: number,
    projectId: string,
    currentAssignee: TestUser,
    newAssignee: TestUser,
) {
    Logger.log("Go to project internal contacts page");
    cy.visit(`projects/${projectId}/internal-contacts`);

    Logger.log("Check the added by user is displayed and click change");
    internalContactsPage.row(3).summaryShows("Added by").hasValue(currentAssignee.username).change("Added by");

    Logger.log("Change the added by user");
    internalContactsPage
        .containsHeading(`Who added this project?`)
        .contains(`URN ${projectUrn}`)
        .hasLabel("Added by")
        .assignTo(newAssignee.username)
        .clickButton("Continue");

    Logger.log("Check the added by user is updated");
    internalContactsPage
        .containsSuccessBannerWithMessage("Project has been updated successfully")
        .row(3)
        .summaryShows("Added by")
        .hasValue(newAssignee.username)
        .hasEmailLink(newAssignee.email);
}
