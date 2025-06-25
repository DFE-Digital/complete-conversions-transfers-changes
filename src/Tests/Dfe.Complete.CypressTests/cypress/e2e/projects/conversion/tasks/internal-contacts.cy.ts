import { ProjectBuilder } from "cypress/api/projectBuilder";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { cypressUser, rdoLondonUser, regionalCaseworkerTeamLeaderUser } from "cypress/constants/cypressConstants";
import internalContactsPage from "cypress/pages/projects/projectDetails/internalContactsPage";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;
const schoolName = "Whitchurch Primary School";

describe("Internal contacts page: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
    });
    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks`);
    });
    it("Should display the user, team and added by details", () => {
        Logger.log("Go to the internal contacts section");
        projectDetailsPage.navigateTo("Internal contacts").containsSubHeading("Internal Contacts");

        Logger.log("Check the user, team and added by details are displayed");
        internalContactsPage
            .inOrder()
            .summaryShows("Assigned to user")
            .hasValue(cypressUser.username)
            .hasEmailLink(cypressUser.email)
            .hasChangeLink(`/projects/${projectId}/internal-contacts/assigned-user/edit`)
            .summaryShows("Assigned to team")
            .hasChangeLink(`/projects/${projectId}/internal-contacts/team/edit`)
            .hasValue("London")
            .summaryShows("Added by")
            .hasValue(cypressUser.username)
            .hasEmailLink(cypressUser.email);
    });

    it("Should cancel the assigned user change", () => {
        cy.visit(`projects/${projectId}/internal-contacts`);

        Logger.log("Check the assigned user is displayed and click change");
        internalContactsPage
            .inOrder()
            .summaryShows("Assigned to user")
            .hasValue(cypressUser.username)
            .change("Assigned to user");

        Logger.log("Click cancel on the change assigned user page");
        internalContactsPage
            .containsHeading(`Change assigned person for ${schoolName}`)
            .contains(`URN ${project.urn.value}`)
            .assignTo(rdoLondonUser.username)
            .clickLink("Cancel");

        Logger.log("Check the assigned user is not changed");
        internalContactsPage.inOrder().summaryShows("Assigned to user").hasValue(cypressUser.username);
    });

    it("Should change the assigned user of the project", () => {
        cy.visit(`projects/${projectId}/internal-contacts`);

        Logger.log("Check the assigned user is displayed and click change");
        internalContactsPage
            .inOrder()
            .summaryShows("Assigned to user")
            .hasValue(cypressUser.username)
            .change("Assigned to user");

        Logger.log("Change the assigned user");
        internalContactsPage
            .containsHeading(`Change assigned person for ${schoolName}`)
            .contains(`URN ${project.urn.value}`)
            .hasLabel("Assign to")
            .assignTo(rdoLondonUser.username)
            .clickButton("Continue");

        Logger.log("Check the assigned user is updated");
        internalContactsPage
            .containsSuccessBannerWithMessage("Project has been assigned successfully")
            .inOrder()
            .summaryShows("Assigned to user")
            .hasValue(rdoLondonUser.username)
            .hasEmailLink(rdoLondonUser.email);
    });

    it("Should change the assigned team of the project", () => {
        cy.visit(`projects/${projectId}/internal-contacts`);

        Logger.log("Check the assigned team is displayed and click change");
        internalContactsPage.row(2).summaryShows("Assigned to team").hasValue("London").change("Assigned to team");

        Logger.log("Change the assigned team");
        internalContactsPage
            .containsHeading(`Change assigned team for ${schoolName}`)
            .contains(`URN ${project.urn.value}`)
            .selectTeam("North East")
            .clickButton("Continue");

        Logger.log("Check the assigned team is updated");
        internalContactsPage
            .containsSuccessBannerWithMessage("Project has been assigned to team successfully")
            .row(2)
            .summaryShows("Assigned to team")
            .hasValue("North East");
    });

    it("Should NOT be able to change the added by user of the project", () => {
        cy.visit(`projects/${projectId}/internal-contacts`);

        Logger.log("Check the added by user is displayed and click change");
        internalContactsPage.row(3).summaryShows("Added by").hasValue(cypressUser.username).hasNoChangeLink("Added by");

        cy.visit(`/projects/${projectId}/internal-contacts/added-by-user/edit`).notAuthorisedToPerformAction();
    });

    it("Should not be able to assign project to a user that cannot create/assign projects", () => {
        Logger.log("Go to project internal contacts page and change assigned user");
        cy.visit(`projects/${projectId}/internal-contacts`);
        internalContactsPage.change("Assigned to user");

        Logger.log("Change the assigned user to a user that cannot create/assign projects");
        internalContactsPage
            .containsHeading(`Change assigned person for ${schoolName}`)
            .contains(`URN ${project.urn.value}`)
            .hasLabel("Assign to")
            .assignToInvalidUser(regionalCaseworkerTeamLeaderUser.username)
            .contains("No results found");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
