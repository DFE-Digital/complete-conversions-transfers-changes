import navBar from "../pages/navBar";
import allProjects from "../pages/projects/allProjects";
import yourTeamProjects from "../pages/projects/yourTeamProjects";
import { Logger } from "cypress/common/logger";
import internalContactsPage from "cypress/pages/projects/projectDetails/internalContactsPage";
import { TestUser } from "cypress/constants/TestUser";
import notePage from "cypress/pages/projects/projectDetails/notePage";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { userToEdit } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";

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
    notePage.doesntContainButton("Add note");
    cy.visit(`/projects/${projectId}/notes/new`).notAuthorisedToPerformAction();
}

export function shouldNotBeAbleToAddAProjectTaskNote(projectId: string) {
    cy.visit(`/projects/${projectId}/tasks/handover`);
    notePage.doesntContain("Add a new task note");
    cy.visit(`/projects/${projectId}/notes/new?task_identifier=handover`).notAuthorisedToPerformAction();
}

export function shouldNotHaveAccessToViewAddEditUsers() {
    cy.visit("/service-support/users").notAuthorisedToPerformAction();
    cy.visit("/service-support/users/new").notAuthorisedToPerformAction();
    cy.visit(`/service-support/users/${userToEdit.id}/edit`).notAuthorisedToPerformAction();
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
        let responseStatus = 200;

        cy.intercept("GET", url, (req) => {
            req.continue((res) => {
                responseStatus = res.statusCode;
            });
        });

        cy.visit(url, { failOnStatusCode: false }).then(() => {
            if (responseStatus >= 500) {
                Logger.log(`Skipping accessibility check for URL (status ${responseStatus}): ${url}`);
            } else {
                Logger.log("Executing accessibility check for URL: " + url);
                cy.executeAccessibilityTests();
            }
        });
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

// confirm the contacts tasks
export function shouldBeAbleToConfirmContact(projectId: string, contactName: string, task: string, taskName: string) {
    cy.visit(`projects/${projectId}/tasks/${task}`);
    Logger.log("Selecting the contact and saving");
    taskPage.hasCheckboxLabel(contactName).tick().saveAndReturn();

    Logger.log("Verifying that the selection was saved correctly");
    taskListPage.hasTaskStatusCompleted(taskName);
}

export function shouldSeeAddContactButtonIfNoContactExists(projectId: string, task: string) {
    cy.visit(`projects/${projectId}/tasks/${task}`);
    taskPage.hasButton("Add a contact");
}

export function shouldNotSeeSaveAndReturnButtonForAnotherUsersProject(projectId: string, task: string) {
    cy.visit(`projects/${projectId}/tasks/${task}`);
    taskPage.noSaveAndReturnExists().doesntContain("Add a contact");
}
