import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import changeDatePage from "cypress/pages/projects/tasks/changeDatePage";
import {
    getDisplayDateString,
    getMonthNumber,
    getSignificantDateString,
    getYearNumber,
} from "cypress/support/formatDate";
import validationComponent from "cypress/pages/validationComponent";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskHelper from "cypress/api/taskHelper";

const inSixMonthsSignificantDate = getSignificantDateString(6);
const inSixMonthsMonth = getMonthNumber(6);
const inSixMonthsYear = getYearNumber(6);
const inThreeMonthsDate = getDisplayDateString(3);
const inThreeMonthsMonth = getMonthNumber(3);
const inThreeMonthsYear = getYearNumber(3);
const inNineMonthsMonth = getMonthNumber(9);
const inNineMonthsYear = getYearNumber(9);
const inNineMonthsDate = getDisplayDateString(9);

const confirmedDateProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversion.stChads,
    provisionalConversionDate: inSixMonthsSignificantDate,
});
let confirmedDateProjectId: string;
const confirmedDateSchoolName = "St Chad's Catholic Primary School";

const provisionalDateProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversion.jessons,
});
let provisionalDateProjectId: string;

const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversion.cradley,
});
let otherUserProjectId: string;

describe("Change the conversion date tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(confirmedDateProject.urn);
        projectRemover.removeProjectIfItExists(provisionalDateProject.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateMatConversionProject(confirmedDateProject).then((response) => {
            confirmedDateProjectId = response.value;
            taskHelper.updateExternalStakeholderKickOff(
                confirmedDateProjectId,
                "completed",
                inSixMonthsSignificantDate,
            );
        });
        projectApi
            .createAndUpdateMatConversionProject(provisionalDateProject)
            .then((response) => (provisionalDateProjectId = response.value));
        projectApi.createAndUpdateMatConversionProject(otherUserProject, rdoLondonUser).then((response) => {
            otherUserProjectId = response.value;
            taskHelper.updateExternalStakeholderKickOff(otherUserProjectId, "completed", inSixMonthsSignificantDate);
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("Should display error when trying to change conversion date to same date", () => {
        cy.visit(`projects/${confirmedDateProjectId}/date-history`);

        Logger.log("Click 'Change conversion date' button");
        projectDetailsPage.clickButton("Change conversion date");

        Logger.log("Enter same date as current conversion date");
        changeDatePage.enterDate(inSixMonthsMonth, inSixMonthsYear).saveAndContinue();

        validationComponent.hasLinkedValidationError(
            "The new date cannot be the same as the current date. Check you have entered the correct date.",
        );
    });

    it("Should be able to change conversion date to an earlier date for your project that has the conversion date confirmed", () => {
        cy.visit(`projects/${confirmedDateProjectId}/tasks`);

        Logger.log("Click 'Change conversion date' button");
        projectDetailsPage.clickButton("Change conversion date");

        Logger.log("Enter new date");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Conversion date")
            .contains("Enter new conversion date")
            .enterDate(inThreeMonthsMonth, inThreeMonthsYear)
            .saveAndContinue();

        Logger.log("Enter reasons for change");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Reasons for conversion date change")
            .contains(`The new proposed conversion date is ${inThreeMonthsDate}`)
            .contains("Reasons for date change")
            .selectReasonWithDetails(
                "Project is progressing faster than expected",
                "We are ready to convert sooner than planned.",
            )
            .selectReasonWithDetails("Correcting an error", "The previous date was incorrect.")
            .saveAndContinue();

        Logger.log("Confirmation page shows the new conversion date");
        projectDetailsPage
            .contains("Conversion date changed")
            .contains(
                `You have changed the conversion date for ${confirmedDateSchoolName}, URN ${confirmedDateProject.urn}.`,
            )
            .containsSubHeading("New conversion date")
            .contains(`The new conversion date is ${inThreeMonthsDate}.`);
    });

    it("Should be able to change conversion date to a later date for your project that has the conversion date confirmed", () => {
        cy.visit(`projects/${confirmedDateProjectId}/tasks`);

        Logger.log("Click 'Change conversion date' button");
        projectDetailsPage.clickButton("Change conversion date");

        Logger.log("Enter new date");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Conversion date")
            .contains("Enter new conversion date")
            .enterDate(inNineMonthsMonth, inNineMonthsYear)
            .saveAndContinue();

        Logger.log("Enter reasons for change");
        changeDatePage
            .contains(confirmedDateSchoolName)
            .containsHeading("Reasons for conversion date change")
            .contains(`The new proposed conversion date is ${inNineMonthsDate}.`)
            .contains("Reasons for date change")
            .selectReasonWithDetails("Incoming trust", "The trust we are joining has requested a later date.")
            .selectReasonWithDetails("School", "The school needs more time to prepare for the conversion.")
            .selectReasonWithDetails("Local Authority", "The local authority has requested a later date.")
            .selectReasonWithDetails("Diocese", "The diocese has requested a later date.")
            .selectReasonWithDetails(
                "TuPE (Transfer of Undertakings Protection Employment rights)",
                "The TuPE process requires more time.",
            )
            .selectReasonWithDetails("Pensions", "Pension arrangements need to be finalised before the conversion.")
            .selectReasonWithDetails("Union", "Unions have requested a later date for the conversion.")
            .selectReasonWithDetails("Negative press coverage", "Negative press coverage has affected the timeline.")
            .selectReasonWithDetails("Governance", "Governance issues need to be resolved before the conversion.")
            .selectReasonWithDetails("Finance", "Financial arrangements need to be finalised before the conversion.")
            .selectReasonWithDetails("Viability", "The school's viability needs to be assessed before the conversion.")
            .selectReasonWithDetails("Land", "Land issues need to be resolved before the conversion.")
            .selectReasonWithDetails("Buildings", "Building works need to be completed before the conversion.")
            .selectReasonWithDetails("Legal Documents", "Legal documents need to be finalised before the conversion.")
            .selectReasonWithDetails("Correcting an error", "The previous date was incorrect.")
            .selectReasonWithDetails("Voluntary deferral", "Being voluntarily deferred for strategic reasons.")
            .selectReasonWithDetails("Advisory board conditions not met", "Advisory board conditions not yet met.")
            .saveAndContinue();

        Logger.log("Confirmation page shows the new conversion date");
        projectDetailsPage
            .contains("Conversion date changed")
            .contains(
                `You have changed the conversion date for ${confirmedDateSchoolName}, URN ${confirmedDateProject.urn}.`,
            )
            .containsSubHeading("New conversion date")
            .contains(`The new conversion date is ${inNineMonthsDate}.`);
    });

    it("Should NOT see 'change conversion date' button for your project that has a provisional date", () => {
        cy.visit(`projects/${provisionalDateProjectId}/tasks`);

        Logger.log("Assert that the 'change conversion date' button is not visible");
        projectDetailsPage.doesntContain("Change conversion date");
    });

    it("Should NOT see the 'change conversion date' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/date-history`);

        Logger.log("Assert that the 'change conversion date' button is not visible");
        projectDetailsPage.doesntContain("Change conversion date");
    });

    it("Should display error when trying to change conversion date to a past date", () => {
        cy.visit(`projects/${confirmedDateProjectId}/date-history`);

        Logger.log("Click 'Change conversion date' button");
        projectDetailsPage.clickButton("Change conversion date");

        Logger.log("Enter past date");
        changeDatePage.enterDate(1, 2025).saveAndContinue();

        validationComponent.hasLinkedValidationError("The Significant date cannot be in the past");
    });

    it("Should NOT be able to submit a change conversion date request with no reasons selected", () => {
        cy.visit(`projects/${confirmedDateProjectId}/tasks`);

        Logger.log("Click 'Change conversion date' button");
        projectDetailsPage.clickButton("Change conversion date");

        Logger.log("Enter new date");
        changeDatePage.enterDate(getMonthNumber(12), getYearNumber(12)).saveAndContinue();

        Logger.log("Click continue without selecting any reasons for change");
        changeDatePage.saveAndContinue();

        validationComponent.hasErrorMessage("You must choose at least one reason");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
