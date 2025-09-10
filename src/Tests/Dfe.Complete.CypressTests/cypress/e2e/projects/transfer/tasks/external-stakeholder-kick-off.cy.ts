import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import {
    getDisplayDateString,
    getMonthNumber,
    getSignificantDateString,
    getYearNumber,
    significateDateToDisplayDate,
} from "cypress/support/formatDate";
import stakeholderKickOffTaskPage from "cypress/pages/projects/tasks/stakeholderKickOffTaskPage";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import validationComponent from "cypress/pages/validationComponent";

const project = ProjectBuilder.createTransferProjectRequest({
    isSignificantDateProvisional: true,
});
let projectId: string;
const project2 = ProjectBuilder.createTransferFormAMatProjectRequest({
    significantDate: getSignificantDateString(12),
    isSignificantDateProvisional: true,
    urn: { value: 105602 },
});
let project2Id: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    isSignificantDateProvisional: true,
    userAdId: rdoLondonUser.adId,
    urn: { value: 105603 },
});
let otherUserProjectId: string;

describe("Transfers tasks - External stakeholder kick-off", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${otherUserProject.urn.value}`);
        projectApi.createTransferProject(project).then((createResponse) => {
            projectId = createResponse.value;
        });
        projectApi.createMatTransferProject(project2).then((createResponse) => {
            project2Id = createResponse.value;
        });
        projectApi.createMatTransferProject(otherUserProject).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/stakeholder_kick_off`);
    });

    // bug 236611
    it.skip("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Send introductory emails")
            .clickDropdown("What to include in introductory emails")
            .hasGuidance("You can choose an email template (opens in new tab) to help you")
            .hasCheckboxLabel("Send invites to the kick-off meeting or call")
            .clickDropdown("How to arrange the kick-off meeting")
            .hasGuidance("Once the trusts have got back to you with a suitable date and list of attendees")
            .hasGuidance("Some trusts may prefer to have a one-to-one call with you")

            .hasCheckboxLabel("Host the kick-off meeting or call")
            .clickDropdown("What to talk about in the meeting")
            .hasGuidance("You can use the transfer checklist (opens in new tab) to guide the conversation");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the all checkboxes and confirm date and save");
        stakeholderKickOffTaskPage
            .hasCheckboxLabel("Send introductory emails")
            .tick()
            .hasCheckboxLabel("Send invites to the kick-off meeting or call")
            .tick()
            .hasCheckboxLabel("Host the kick-off meeting or call")
            .tick()
            .enterSignificantDate(10, 2027)
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("External stakeholder kick-off")
            .selectTask("External stakeholder kick-off");

        Logger.log("Unselect all all checkboxes and save");
        stakeholderKickOffTaskPage
            .hasCheckboxLabel("Send introductory emails")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Send invites to the kick-off meeting or call")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Host the kick-off meeting or call")
            .isTicked()
            .untick()
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("External stakeholder kick-off")
            .selectTask("External stakeholder kick-off");
        stakeholderKickOffTaskPage
            .hasCheckboxLabel("Send introductory emails")
            .isUnticked()
            .hasCheckboxLabel("Send invites to the kick-off meeting or call")
            .isUnticked()
            .hasCheckboxLabel("Host the kick-off meeting or call")
            .isUnticked();
    });

    // bug 235385
    it.skip("Should NOT be able to set a transfer date in the past", () => {
        cy.visit(`projects/${project2Id}/tasks/stakeholder_kick_off`);
        stakeholderKickOffTaskPage.enterSignificantDate(1, 2020).saveAndReturn();
        validationComponent.hasLinkedValidationError("The Significant date cannot be in the past");
    });

    it("Should only be able to confirm the transfer date once", () => {
        cy.visit(`projects/${project2Id}/tasks`);
        projectDetailsPage.hasProvisionalDateTag();

        Logger.log("Confirm the transfer date");
        taskListPage.selectTask("External stakeholder kick-off");
        stakeholderKickOffTaskPage.enterSignificantDate(getMonthNumber(1), getYearNumber(1)).saveAndReturn();

        Logger.log("Confirm the transfer date can no longer be changed");
        projectDetailsPage.doesntHaveProvisionalDateTag();
        taskListPage
            .hasTaskStatusInProgress("External stakeholder kick-off")
            .selectTask("External stakeholder kick-off");
        stakeholderKickOffTaskPage
            .contains(`The transfer date has been confirmed and is currently ${getDisplayDateString(1)}`)
            .contains(`The provisional transfer date was ${significateDateToDisplayDate(project2.significantDate)}`)
            .contains("Any further changes to the transfer date must be recorded on the main project screen.");
    });

    it("Should NOT see the not applicable option for this task", () => {
        taskPage.noNotApplicableOptionExists();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/stakeholder_kick_off`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
