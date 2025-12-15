import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewReportsLandingPage,
    shouldNotBeAbleToAddAProjectNote,
    shouldNotBeAbleToAddAProjectTaskNote,
    shouldNotHaveAccessToViewYourProjectsSections,
    shouldNotHaveAccessToViewYourTeamProjectsSections,
} from "cypress/support/reusableTests";
import navBar from "cypress/pages/navBar";
import { serviceSupportUser } from "cypress/constants/cypressConstants";
import allProjects from "cypress/pages/projects/allProjects";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";
import editTransferProjectPage from "cypress/pages/projects/edit/editTransferProjectPage";
import internalContactsPage from "cypress/pages/projects/projectDetails/internalContactsPage";
import externalContactsPage from "cypress/pages/projects/projectDetails/externalContactsPage";
import { ContactBuilder } from "cypress/api/contactBuilder";
import contactApi from "cypress/api/contactApi";
import taskHelper from "cypress/api/taskHelper";

const project = ProjectBuilder.createConversionProjectRequest({ urn: urnPool.userCapabilities.longnor });
let projectId: string;
const contact = ContactBuilder.createContactRequest();
const projectToDelete = ProjectBuilder.createConversionProjectRequest({ urn: urnPool.userCapabilities.mountjoy });
let projectToDeleteId: string;
const schoolToDeleteName = "Mountjoy House School";
const projectMat = ProjectBuilder.createConversionFormAMatProjectRequest({ urn: urnPool.userCapabilities.morda });
let projectMatId: string;

before(() => {
    projectRemover.removeProjectIfItExists(project.urn);
    projectRemover.removeProjectIfItExists(projectToDelete.urn);
    projectRemover.removeProjectIfItExists(projectMat.urn);
    projectApi.createAndUpdateConversionProject(project).then((response) => {
        projectId = response.value;
        contact.projectId = { value: projectId };
        contactApi.createContact(contact);
        taskHelper.updateExternalStakeholderKickOff(projectId, "completed", project.provisionalConversionDate);
    });
    projectApi
        .createAndUpdateConversionProject(projectToDelete)
        .then((response) => (projectToDeleteId = response.value));
    projectApi.createAndUpdateMatConversionProject(projectMat).then((response) => (projectMatId = response.value));
});
beforeEach(() => {
    cy.login(serviceSupportUser);
    cy.acceptCookies();
    cy.visit("/");
});
describe("Capabilities and permissions of the service support user - project listings", () => {
    it("Should direct user to all Service support -> without academy URNs after login", () => {
        cy.url().should("include", "/projects/service-support/without-academy-urn");
    });

    it("Should be able to view 'All projects', 'Groups' and 'Service support' sections and filters", () => {
        navBar.ableToView(["All projects", "Groups", "Service support"]);
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
            "Reports",
        ]);
    });

    it("Should NOT be able to view 'Your projects', 'Your team projects' sections", () => {
        navBar.unableToView(["Your projects", "Your team projects"]);
    });

    it("Should NOT have access to view Your projects sections", () => {
        shouldNotHaveAccessToViewYourProjectsSections();
    });

    it("Should NOT have access to view Your team projects sections", () => {
        shouldNotHaveAccessToViewYourTeamProjectsSections();
    });

    it("Should be able to view the reports landing page", () => {
        shouldBeAbleToViewReportsLandingPage();
    });

    it("Should be able to review projects newly handed over from prepare", () => {
        cy.visit("/projects/all/handover");
        cy.contains("Projects to handover");
    });
});
describe("Capabilities and permissions of the service support user - project pages", () => {
    it("Should be able to soft delete a project", () => {
        cy.visit(`/projects/${projectToDeleteId}`);
        taskListPage
            .clickButton("Delete project")
            .containsHeading(`Delete ${schoolToDeleteName}`)
            .clickButton("Delete project")
            .containsSuccessBannerWithMessage("The project was deleted.");
        cy.url().should("include", "/projects/all/in-progress/all");
    });

    it("Should be able to update the significant date of the project", () => {
        cy.visit(`/projects/${projectId}/information`);
        aboutTheProjectPage.hasButton("Change conversion date");
    });

    it("Should be able to update a task for another user's project", () => {
        cy.visit(`/projects/${projectId}/tasks/subleases`);
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .containsSuccessBannerWithMessage("Task updated successfully")
            .hasTaskStatusNotApplicable("Subleases")
            .selectTask("Subleases");
    });

    it("Should NOT be able to add a task note to a project", () => {
        shouldNotBeAbleToAddAProjectTaskNote(projectId);
    });

    it("Should be able to update project details for another user's project", () => {
        cy.visit(`/projects/${projectId}/information`);
        aboutTheProjectPage.change("Group reference number");
        editTransferProjectPage.with2RI("Yes").continue();
        aboutTheProjectPage.containsSuccessBannerWithMessage("Project has been updated successfully");
    });

    it("Should be able to update MAT project TRN", () => {
        cy.visit(`/projects/${projectMatId}/information`);
        aboutTheProjectPage.change("New trust reference number (TRN)");
        editTransferProjectPage.withTrustReferenceNumber("TR01881").continue();
        aboutTheProjectPage.containsSuccessBannerWithMessage("Project has been updated successfully");
    });

    it("Should NOT be able to add a note to a project", () => {
        shouldNotBeAbleToAddAProjectNote(projectId);
    });

    it("Should NOT be able to add external contacts", () => {
        cy.visit(`/projects/${projectId}/external-contacts`);
        externalContactsPage.doesntContainButton(/Add contact/i);
    });

    it("Should be able to edit external contacts", () => {
        cy.visit(`/projects/${projectId}/external-contacts`);
        externalContactsPage.setContactItemByRoleHeading(contact.role!).hasContactItem().editContact();
    });

    it("Should have access to change any of a project's internal contacts", () => {
        cy.visit(`/projects/${projectId}/internal-contacts`);
        internalContactsPage
            .inOrder()
            .summaryShows("Assigned to user")
            .hasChangeLink(`/projects/${projectId}/internal-contacts/assigned-user/edit?returnUrl=internal-contacts`)
            .summaryShows("Assigned to team")
            .hasChangeLink(`/projects/${projectId}/internal-contacts/team/edit`)
            .summaryShows("Added by")
            .hasChangeLink(`/projects/${projectId}/internal-contacts/added-by-user/edit`);
    });
});
