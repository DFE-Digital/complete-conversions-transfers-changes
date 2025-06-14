import projectRemover from "cypress/api/projectRemover";
import homePage from "cypress/pages/homePage";
import newTransferPage from "cypress/pages/projects/new/newTransferPage";
import selectProjectTypePage from "cypress/pages/projects/new/selectProjectTypePage";
import validationComponent from "cypress/pages/validationComponent";
import {
    groupReferenceNumber,
    testTrustName,
    testTrustReferenceNumber,
    ukprn,
    ukprn2,
} from "cypress/constants/stringTestConstants";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const urn = "136730";
const urnMAT = "136731";

describe("Create a new Transfer Project", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(urn);
        projectRemover.removeProjectIfItExists(urnMAT);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit("/");
    });

    // bug 216887
    it.skip("Should be able to create a new Transfer Project", () => {
        homePage.addAProject();

        selectProjectTypePage.selectTransfer().continue();

        newTransferPage
            .withAcademyURN(urn)
            .withOutgoingTrustUKPRN(`${ukprn}`)
            .withIncomingTrustUKPRN(`${ukprn2}`)
            .withGroupReferenceNumber(groupReferenceNumber)
            .withAcademySharepointLink("https://educationgovuk.sharepoint.com")
            .withIncomingTrustSharePointLink("https://educationgovuk.sharepoint.com")
            .withOutgoingTrustSharepointLink("https://educationgovuk.sharepoint.com")
            .withAdvisoryBoardDate("12", "09", "2023")
            .withAdvisoryBoardConditions("Test conditions")
            .withProvisionalTransferDate("09", "2023")
            .with2RI("No")
            .withTransferDueToInadequateOfstedRating("No")
            .withTransferDueToFinancialSafeguardingGovernanceIssues("No")
            .withOutgoingTrustWillCloseAfterTransfer("No")
            .withHandingOverToRCS("No")
            .withHandoverComments("Test comments")
            .continue();

        validationComponent.hasNoValidationErrors();
        cy.get("h2").should("contain", "Task list");
    });

    // bug 216887
    it.skip("Should be able to create a new Form a MAT transfer project", () => {
        homePage.addAProject();

        selectProjectTypePage.selectFormAMATTransfer().continue();

        newTransferPage
            .withAcademyURN(urnMAT)
            .withOutgoingTrustUKPRN(`${ukprn}`)
            .withTrustReferenceNumber(testTrustReferenceNumber)
            .withTrustName(testTrustName)
            .withAcademySharepointLink("https://educationgovuk.sharepoint.com")
            .withIncomingTrustSharePointLink("https://educationgovuk.sharepoint.com")
            .withOutgoingTrustSharepointLink("https://educationgovuk.sharepoint.com")
            .withAdvisoryBoardDate("26", "09", "2024")
            .withAdvisoryBoardConditions("Test conditions")
            .withProvisionalTransferDate("09", "2023")
            .with2RI("Yes")
            .withTransferDueToInadequateOfstedRating("Yes")
            .withTransferDueToFinancialSafeguardingGovernanceIssues("Yes")
            .withOutgoingTrustWillCloseAfterTransfer("Yes")
            .withHandingOverToRCS("Yes")
            .continue();

        validationComponent.hasNoValidationErrors();
        cy.get("h2").should("contain", "Task list");
    });

    it("Should show multiple validation errors when inputting invalid data in Form a MAT Transfer", () => {
        homePage.addAProject();

        selectProjectTypePage.selectFormAMATTransfer().continue();

        newTransferPage
            .withAcademyURN("123")
            .withOutgoingTrustUKPRN("a")
            .withTrustReferenceNumber("abcdef")
            .withTrustName("hi")
            .withAcademySharepointLink("hi")
            .withIncomingTrustSharePointLink("123")
            .withOutgoingTrustSharepointLink("https://example.com")
            .withAdvisoryBoardDate("a", "b", "c")
            .withProvisionalTransferDate("19", "2025")
            .continue();

        validationComponent
            .hasLinkedValidationError("The Urn must be 6 digits long. For example, 123456.")
            .hasLinkedValidationError(
                "The OutgoingUKPRN must be 8 digits long and start with a 1. For example, 12345678.",
            )
            .hasLinkedValidationError("The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234")
            .hasLinkedValidationError("The School or academy SharePoint link must have the https scheme")
            .hasLinkedValidationError("The Incoming trust SharePoint link must have the https scheme")
            .hasLinkedValidationError(
                "Enter Outgoing trust SharePoint link in the correct format. SharePoint links start with 'https://educationgovuk.sharepoint.com' or 'https://educationgovuk-my.sharepoint.com/'",
            )
            .hasLinkedValidationError("Advisory Board Date must be a real date")
            .hasLinkedValidationError("Provisional Transfer Date must be a real date")
            .hasLinkedValidationError("State if the conversion is due to 2RI. Choose yes or no")
            .hasLinkedValidationError("State if the transfer is due to an inadequate Ofsted rating. Choose yes or no")
            .hasLinkedValidationError(
                "State if the transfer is due to financial, safeguarding or governance issues. Choose yes or no",
            )
            .hasLinkedValidationError(
                "State if the outgoing trust will close once this transfer is completed. Choose yes or no",
            )
            .hasLinkedValidationError(
                "State if this project will be handed over to the Regional casework services team. Choose yes or no",
            );
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
