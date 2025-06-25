import navBar from "../pages/navBar";
import allProjects from "../pages/projects/allProjects";
import yourTeamProjects from "../pages/projects/yourTeamProjects";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { currentMonthLong, currentMonthShort } from "cypress/constants/stringTestConstants";
import { Logger } from "cypress/common/logger";
import internalContactsPage from "cypress/pages/projects/projectDetails/internalContactsPage";
import { TestUser } from "cypress/constants/TestUser";

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
