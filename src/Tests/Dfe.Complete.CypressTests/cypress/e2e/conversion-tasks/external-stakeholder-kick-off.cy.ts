import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelper from "cypress/api/taskHelper";
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
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: { value: urnPool.conversionTasks.spen },
    isSignificantDateProvisional: true,
});
let projectId: string;
const project2 = ProjectBuilder.createConversionFormAMatProjectRequest({
    significantDate: getSignificantDateString(12),
    isSignificantDateProvisional: true,
    urn: { value: urnPool.conversionTasks.grylls },
});
let project2Id: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    isSignificantDateProvisional: true,
    userAdId: rdoLondonUser.adId,
    urn: { value: urnPool.conversionTasks.huddersfield },
});
let otherUserProjectId: string;

describe("Conversion tasks - External stakeholder kick-off", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectRemover.removeProjectIfItExists(project2.urn.value);
        projectRemover.removeProjectIfItExists(otherUserProject.urn.value);
        projectApi.createConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
        });
        projectApi.createMatConversionProject(project2).then((createResponse) => {
            project2Id = createResponse.value;
        });
        projectApi.createMatConversionProject(otherUserProject).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/stakeholder_kick_off`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .hasCheckboxLabel("Send introductory emails")
            .clickDropdown("What to include in introductory emails")
            .hasGuidance("You can choose an email template (opens in new tab) to help you")
            .hasCheckboxLabel("Check the local authority proforma")
            .clickDropdown("What to do if you don't have the local authority proforma")
            .hasGuidance("The person who prepared this project for advisory board")
            // bug 236611
            // .hasCheckboxLabel(
            //     `Check the local authority is able to convert the school by the provisional conversion date: ${significateDateToDisplayDate(
            //         project.significantDate,
            //     )}`,
            // )
            // .clickDropdown("What to do if the local authority is not able meet the provisional conversion date")
            // .hasGuidance(
            //     "Tell the school and trust if the local authority cannot complete the conversion by the provisional date.",
            // )
            .hasCheckboxLabel("Send invites to the kick-off meeting or call")
            .clickDropdown("How to arrange the kick-off meeting")
            .hasGuidance("Once the school have got back to you with a suitable date and list of attendees")
            .hasGuidance("Some schools and trusts may prefer to have a one-to-one call with you")
            .hasCheckboxLabel("Host the kick-off meeting or call")
            .clickDropdown("What to talk about in the meeting")
            .hasGuidance("You can use the conversion checklist (opens in new tab) to guide the conversation");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the first checkbox and save");
        taskPage.hasCheckboxLabel("Send introductory email").tick().saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("External stakeholder kick-off")
            .selectTask("External stakeholder kick-off");

        Logger.log("Unselect the first checkbox and save");
        taskPage.hasCheckboxLabel("Send introductory email").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("External stakeholder kick-off")
            .selectTask("External stakeholder kick-off");
        taskPage.hasCheckboxLabel("Send introductory email").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        TaskHelper.updateExternalStakeholderKickOff(projectId, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("External stakeholder kick-off");

        TaskHelper.updateExternalStakeholderKickOff(projectId, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("External stakeholder kick-off");
    });

    // bug 235385
    it.skip("Should NOT be able to set a conversion date in the past", () => {
        cy.visit(`projects/${project2Id}/tasks/stakeholder_kick_off`);
        stakeholderKickOffTaskPage.enterSignificantDate(1, 2020).saveAndReturn();
        validationComponent.hasLinkedValidationError("The Significant date cannot be in the past");
    });

    it("Should only be able to confirm the conversion date once", () => {
        cy.visit(`projects/${project2Id}/tasks`);
        projectDetailsPage.hasProvisionalDateTag();

        Logger.log("Confirm the conversion date");
        taskListPage.selectTask("External stakeholder kick-off");
        stakeholderKickOffTaskPage.enterSignificantDate(getMonthNumber(1), getYearNumber(1)).saveAndReturn();

        Logger.log("Confirm the conversion date can no longer be changed");
        projectDetailsPage.doesntHaveProvisionalDateTag();
        taskListPage
            .hasTaskStatusInProgress("External stakeholder kick-off")
            .selectTask("External stakeholder kick-off");
        stakeholderKickOffTaskPage
            .contains(`The conversion date has been confirmed and is currently ${getDisplayDateString(1)}`)
            .contains(`The provisional conversion date was ${significateDateToDisplayDate(project2.significantDate)}`)
            .contains("Any further changes to the conversion date must be recorded on the main project screen.");
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
