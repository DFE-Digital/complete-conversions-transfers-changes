import { ProjectBuilder } from "cypress/api/projectBuilder";
import { getDisplayDateString, getSignificantDateString, toDisplayDate } from "cypress/support/formatDate";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { cypressUser, rdoLondonUser } from "cypress/constants/cypressConstants";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import dateHistoryPage from "cypress/pages/projects/projectDetails/dateHistoryPage";
import { today } from "cypress/constants/stringTestConstants";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const inSixMonthsSignificantDate = getSignificantDateString(6);
const inSixMonthsDisplayDate = getDisplayDateString(6);
const inThreeMonthsSignificantDate = getSignificantDateString(3);
const inThreeMonthsDisplayDate = getDisplayDateString(3);
const inNineMonthsSignificantDate = getSignificantDateString(9);
const inNineMonthsDisplayDate = getDisplayDateString(9);

const confirmedDateProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: 103886 },
    significantDate: inSixMonthsSignificantDate,
    isSignificantDateProvisional: false,
});

let confirmedDateProjectId: string;

const reasonsForChange1 = {
    AdvisoryBoardConditions: "Advisory board has been established to oversee the conversion process.",
    Buildings: "Building renovations are scheduled to start next month.",
    CorrectingAnError: "Correcting an error in the previous date setting.",
    Diocese: "Diocese has approved the conversion plans and is fully supportive.",
    Finance: "Financial audits have been completed and funding is secured.",
    Governance: "Governance structure has been reviewed and approved by the board.",
    IncomingTrust: "Incoming trust has been selected and is ready to proceed with the conversion.",
    Land: "Land acquisition is complete and all legal documents are in order.",
    LegalDocuments: "All legal documents have been reviewed and signed by the relevant parties.",
    LocalAuthority: "Local authority has been consulted and supports the conversion.",
    NegativePress: "No negative press has been reported regarding the conversion.",
    Pensions: "Pension arrangements have been finalized and approved by the relevant authorities.",
    School: "The school community is fully engaged and supportive of the conversion.",
    Tupe: "TUPE arrangements have been communicated to all staff and are in compliance with legal requirements.",
    Union: "Union representatives have been consulted and are supportive of the conversion.",
    Viability: "The school's viability has been assessed and confirmed as strong, ensuring a successful conversion.",
    VoluntaryDeferral: "Voluntary deferral has been requested to allow more time for preparations.",
};

const reasonsForChange2 = {
    ProgressingFasterThanExpected:
        "The school is ready to convert ahead of schedule due to excellent preparation work.",
    CorrectingAnError: "The original date was set incorrectly in the initial planning phase.",
};

describe("View the conversion date history tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${confirmedDateProject.urn.value}`);
        projectApi.createMatConversionProject(confirmedDateProject).then((response) => {
            confirmedDateProjectId = response.value;
            projectApi.updateProjectSignificantDate(
                confirmedDateProjectId,
                inThreeMonthsSignificantDate,
                reasonsForChange2,
                rdoLondonUser.id,
            );
            projectApi.updateProjectSignificantDate(
                confirmedDateProjectId,
                inNineMonthsSignificantDate,
                reasonsForChange1,
                cypressUser.id,
            );
        });
    });

    beforeEach(() => {
        cy.login();
        cy.visit(`/projects/${confirmedDateProjectId}/tasks`);
    });

    it("Should be able to view the conversion date history for a project", () => {
        Logger.log("Go to Conversion date history section");
        projectDetailsPage
            .navigateTo("Conversion date history")
            // .containsSubHeading("Current confirmed conversion date") // bug 231908
            // .contains(`The current confirmed conversion date is ${inSixMonthsDisplayDate}.`)
            .containsSubHeading("Conversion date history");

        Logger.log("Check the history of conversion dates");
        dateHistoryPage
            .inOrder()
            .hasDateHistoryItem()
            .hasSubHeading(toDisplayDate(today))
            .summaryShows("Changed by")
            .hasValue(cypressUser.email)
            .summaryShows("Date changed to")
            .hasValue(inNineMonthsDisplayDate)
            .summaryShows("Date changed from")
            .hasValue(inThreeMonthsDisplayDate)
            .summaryShows("Reason for new date")
            .hasReasonNewDate("Advisory board conditions not met", reasonsForChange1.AdvisoryBoardConditions)
            .hasReasonNewDate("Buildings", reasonsForChange1.Buildings)
            .hasReasonNewDate("Correcting an error", reasonsForChange1.CorrectingAnError)
            .hasReasonNewDate("Diocese", reasonsForChange1.Diocese)
            .hasReasonNewDate("Finance", reasonsForChange1.Finance)
            .hasReasonNewDate("Governance", reasonsForChange1.Governance)
            .hasReasonNewDate("Incoming trust", reasonsForChange1.IncomingTrust)
            .hasReasonNewDate("Land", reasonsForChange1.Land)
            .hasReasonNewDate("Legal Documents", reasonsForChange1.LegalDocuments)
            .hasReasonNewDate("Local Authority", reasonsForChange1.LocalAuthority)
            .hasReasonNewDate("Negative press coverage", reasonsForChange1.NegativePress)
            .hasReasonNewDate("Pensions", reasonsForChange1.Pensions)
            .hasReasonNewDate("School", reasonsForChange1.School)
            .hasReasonNewDate("TuPE (Transfer of Undertakings Protection Employment rights)", reasonsForChange1.Tupe)
            .hasReasonNewDate("Union", reasonsForChange1.Union)
            .hasReasonNewDate("Viability", reasonsForChange1.Viability)
            .hasReasonNewDate("Voluntary deferral", reasonsForChange1.VoluntaryDeferral)
            .hasDateHistoryItem()
            .hasSubHeading(toDisplayDate(today))
            .summaryShows("Changed by")
            .hasValue(rdoLondonUser.email)
            .summaryShows("Date changed to")
            .hasValue(inThreeMonthsDisplayDate)
            .summaryShows("Date changed from")
            .hasValue(inSixMonthsDisplayDate)
            .summaryShows("Reason for new date")
            .hasReasonNewDate("Correcting an error", reasonsForChange2.CorrectingAnError)
            .hasReasonNewDate(
                "Project is progressing faster than expected",
                reasonsForChange2.ProgressingFasterThanExpected,
            );
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
