import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import changeDatePage from "cypress/pages/projects/tasks/changeDatePage";
import {
    getDisplayDateString,
    getMonthNumber,
    getSignificantDateString,
    getYearNumber,
} from "cypress/support/formatDate";
import { urnPool } from "cypress/constants/testUrns";

const inSixMonthsSignificantDate = getSignificantDateString(6);
const inThreeMonthsDate = getDisplayDateString(3);
const inThreeMonthsMonth = getMonthNumber(3);
const inThreeMonthsYear = getYearNumber(3);
const inNineMonthsMonth = getMonthNumber(9);
const inNineMonthsYear = getYearNumber(9);
const inNineMonthsDate = getDisplayDateString(9);

const confirmedDateProject = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.transfer.abbey },
    significantDate: inSixMonthsSignificantDate,
    isSignificantDateProvisional: false,
});
let confirmedDateProjectId: string;
const confirmedDateSchoolName = "Abbey College Manchester";

const provisionalDateProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.transfer.priory },
    isSignificantDateProvisional: true,
});
let provisionalDateProjectId: string;

const otherUserProject = ProjectBuilder.createTransferProjectRequest({
    urn: { value: urnPool.transfer.prees },
    isSignificantDateProvisional: false,
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;
describe("Change the transfer date tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(confirmedDateProject.urn.value);
        projectRemover.removeProjectIfItExists(provisionalDateProject.urn.value);
        projectRemover.removeProjectIfItExists(otherUserProject.urn.value);
        projectApi
            .createTransferProject(confirmedDateProject)
            .then((response) => (confirmedDateProjectId = response.value));
        projectApi
            .createMatTransferProject(provisionalDateProject)
            .then((response) => (provisionalDateProjectId = response.value));
        projectApi.createTransferProject(otherUserProject).then((response) => (otherUserProjectId = response.value));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should fail for debugging purposes", () => {
        cy.contains("something that does not exist");
    })

    it("Should be able to change transfer date to an earlier date for your project that has the transfer date confirmed", () => {
        cy.visit(`projects/${confirmedDateProjectId}/tasks`);

        Logger.log("Click 'Change transfer date' button");
        projectDetailsPage.clickButton("Change transfer date");

        Logger.log("Enter new date");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Transfer date")
            .contains("Enter new transfer date")
            .enterDate(inThreeMonthsMonth, inThreeMonthsYear)
            .saveAndContinue();

        Logger.log("Enter reasons for change");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Reasons for transfer date change")
            .contains(`The new proposed transfer date is ${inThreeMonthsDate}`)
            .contains("Reasons for date change")
            .selectReasonWithDetails(
                "Project is progressing faster than expected",
                "We are ready to transfer sooner than planned.",
            )
            .selectReasonWithDetails("Correcting an error", "The previous date was incorrect.")
            .saveAndContinue();

        Logger.log("Confirmation page shows the new transfer date");
        projectDetailsPage
            .contains("Transfer date changed")
            .contains(
                `You have changed the transfer date for ${confirmedDateSchoolName}, URN ${confirmedDateProject.urn.value}.`,
            )
            .containsSubHeading("New transfer date")
            .contains(`The new transfer date is ${inThreeMonthsDate}.`);
    });

    it("Should be able to change transfer date to a later date for your project that has the transfer date confirmed", () => {
        cy.visit(`projects/${confirmedDateProjectId}/tasks`);

        Logger.log("Click 'Change transfer date' button");
        projectDetailsPage.clickButton("Change transfer date");

        Logger.log("Enter new date");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Transfer date")
            .contains("Enter new transfer date")
            .enterDate(inNineMonthsMonth, inNineMonthsYear)
            .saveAndContinue();

        Logger.log("Enter reasons for change");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Reasons for transfer date change")
            .contains(`The new proposed transfer date is ${inNineMonthsDate}.`)
            .contains("Reasons for date change")
            .selectReasonWithDetails("Incoming trust", "The trust we are joining has requested a later date.")
            .selectReasonWithDetails("Outgoing trust", "The trust we are leaving has requested a later date.")
            .selectReasonWithDetails("Academy", "The academy needs more time to prepare for the transfer.")
            .selectReasonWithDetails("Local Authority", "The local authority has requested a later date.")
            .selectReasonWithDetails("Diocese", "The diocese has requested a later date.")
            .selectReasonWithDetails(
                "TuPE (Transfer of Undertakings Protection Employment rights)",
                "The TuPE process requires more time.",
            )
            .selectReasonWithDetails("Pensions", "Pension arrangements need to be finalised before the transfer.")
            .selectReasonWithDetails("Union", "Unions have requested a later date for the transfer.")
            .selectReasonWithDetails("Negative press coverage", "Negative press coverage has affected the timeline.")
            .selectReasonWithDetails("Governance", "Governance issues need to be resolved before the transfer.")
            .selectReasonWithDetails("Finance", "Financial arrangements need to be finalised before the transfer.")
            .selectReasonWithDetails("Viability", "The school's viability needs to be assessed before the transfer.")
            .selectReasonWithDetails("Land", "Land issues need to be resolved before the transfer.")
            .selectReasonWithDetails("Buildings", "Building works need to be completed before the transfer.")
            .selectReasonWithDetails("Legal Documents", "Legal documents need to be finalised before the transfer.")
            .selectReasonWithDetails(
                "Commercial transfer agreement",
                "The commercial transfer agreement needs to be finalised.",
            )
            .selectReasonWithDetails("Correcting an error", "The previous date was incorrect.")
            .selectReasonWithDetails("Voluntary deferral", "Being voluntarily deferred for strategic reasons.")
            .selectReasonWithDetails("Advisory board conditions not met", "Advisory board conditions not yet met.")
            .saveAndContinue();

        Logger.log("Confirmation page shows the new transfer date");
        projectDetailsPage
            .contains("Transfer date changed")
            .contains(
                `You have changed the transfer date for ${confirmedDateSchoolName}, URN ${confirmedDateProject.urn.value}.`,
            )
            .containsSubHeading("New transfer date")
            .contains(`The new transfer date is ${inNineMonthsDate}.`);
    });

    it("Should NOT see 'change transfer date' button for your project that has a provisional date", () => {
        cy.visit(`projects/${provisionalDateProjectId}/tasks`);

        Logger.log("Assert that the 'change transfer date' button is not visible");
        projectDetailsPage.doesntContain("Change transfer date");
    });

    it("Should NOT see the 'change transfer date' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/date-history`);

        Logger.log("Assert that the 'change transfer date' button is not visible");
        projectDetailsPage.doesntContain("Change transfer date");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
