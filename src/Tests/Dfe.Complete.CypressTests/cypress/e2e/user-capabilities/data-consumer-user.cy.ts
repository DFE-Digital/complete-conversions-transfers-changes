import { beforeEach } from "mocha";
import {
    checkAccessibilityAcrossPages,
    shouldBeAbleToViewReportsLandingPage,
    shouldNotBeAbleToAddAProjectNote,
    shouldNotBeAbleToAddAProjectTaskNote,
    shouldNotBeAbleToSoftDeleteAProject,
    shouldNotHaveAccessToViewAddEditUsers,
    shouldNotHaveAccessToViewConversionURNsPage,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldNotHaveAccessToViewLocalAuthorities,
    shouldNotHaveAccessToViewYourProjectsSections,
    shouldNotHaveAccessToViewYourTeamProjectsSections,
} from "cypress/support/reusableTests";
import { dataConsumerUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import allProjects from "cypress/pages/projects/allProjects";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import taskHelper from "cypress/api/taskHelper";
import internalContactsPage from "cypress/pages/projects/projectDetails/internalContactsPage";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import aboutTheProjectPage from "cypress/pages/projects/projectDetails/aboutTheProjectPage";
import contactApi from "cypress/api/contactApi";
import { ContactBuilder } from "cypress/api/contactBuilder";
import externalContactsPage from "cypress/pages/projects/projectDetails/externalContactsPage";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.userCapabilities.longnor,
    provisionalConversionDate: "2027-04-01",
});
let projectId: string;

before(() => {
    projectRemover.removeProjectIfItExists(project.urn);
    projectApi.createAndUpdateConversionProject(project).then((response) => {
        projectId = response.value;
        taskHelper.updateExternalStakeholderKickOff(projectId, "completed", "2027-04-01");
        contactApi.createContact(ContactBuilder.createContactRequest({ projectId: { value: projectId } }));
    });
});
beforeEach(() => {
    cy.login(dataConsumerUser);
    cy.acceptCookies();
    cy.visit("/");
});

describe("Capabilities and permissions of the data consumer user - listing pages", () => {
    it("Should direct user to all projects in progress after login", () => {
        cy.url().should("include", "/projects/all/in-progress/all");
    });

    it("Should be able to view All projects section, with all the expected filters", () => {
        navBar.unableToViewTheNavBar();
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

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT have access to view Your projects sections", () => {
        shouldNotHaveAccessToViewYourProjectsSections();
    });

    it("Should NOT have access to view Your team projects sections", () => {
        shouldNotHaveAccessToViewYourTeamProjectsSections();
    });

    it("Should NOT have access to view, add or edit users", () => {
        shouldNotHaveAccessToViewAddEditUsers();
    });

    it("Should be able to view the reports landing page", () => {
        shouldBeAbleToViewReportsLandingPage();
    });

    it("Should NOT be able to view and edit local authorities", () => {
        shouldNotHaveAccessToViewLocalAuthorities();
    });

    it("Should NOT be able to view conversion URNs", () => {
        shouldNotHaveAccessToViewConversionURNsPage(projectId);
    });
});
describe("Capabilities and permissions of the data consumer user - project pages", () => {
    it("Should NOT be able to soft delete projects", () => {
        shouldNotBeAbleToSoftDeleteAProject(projectId);
    });

    it("Should NOT be able to update the significant date of the project", () => {
        cy.visit(`/projects/${projectId}/information`);
        aboutTheProjectPage.doesntContainButton("Change conversion date");
    });

    it("Should NOT be able to update a project task", () => {
        cy.visit(`projects/${projectId}/tasks/subleases`);
        taskPage.noSaveAndReturnExists();
    });

    it("Should NOT be able to add a task note to a project", () => {
        shouldNotBeAbleToAddAProjectTaskNote(projectId);
    });

    it("Should NOT be able to update project details", () => {
        cy.visit(`/projects/${projectId}/information`);
        aboutTheProjectPage.linkDoesNotExist("Change");
    });

    it("Should NOT be able to add a note to a project", () => {
        shouldNotBeAbleToAddAProjectNote(projectId);
    });

    it("Should NOT be able to add external contacts", () => {
        cy.visit(`/projects/${projectId}/external-contacts`);
        externalContactsPage.doesntContainButton(/Add contact/i);
    });

    it("Should NOT be able to edit external contacts", () => {
        cy.visit(`/projects/${projectId}/external-contacts`);
        externalContactsPage.linkDoesNotExist("Change");
    });

    it("Should NOT be able to update internal contacts", () => {
        cy.visit(`/projects/${projectId}/internal-contacts`);
        internalContactsPage.linkDoesNotExist("Change");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
