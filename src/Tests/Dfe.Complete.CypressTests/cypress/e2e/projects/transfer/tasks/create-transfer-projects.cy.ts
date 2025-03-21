import projectRemover from "../../../../api/projectRemover";
import homePage from "../../../../pages/homePage";
import selectProjectTypePage from "../../../../pages/projects/new/selectProjectTypePage";
import newTransferPage from "../../../../pages/projects/new/newTransferPage";
import validationComponent from "../../../../pages/validationComponent";

const urn = "136730";
const urnMAT = "136731";

describe("Create a new Transfer Project", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(urn);
        // projectRemover.removeProjectIfItExists(urnMAT); // skip bug 202345
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should be able to create a new Transfer Project", () => {
        homePage.addAProject();

        selectProjectTypePage.selectTransfer().continue();

        newTransferPage
            .withAcademyURN(urn)
            .withOutgoingTrustUKPRN("10058682")
            .withIncomingTrustUKPRN("10058689")
            .withGroupReferenceNumber("GRP_00000006")
            .withAcademySharepointLink("https://educationgovuk.sharepoint.com")
            .withIncomingTrustSharePointLink("https://educationgovuk.sharepoint.com")
            .withOutgoingTrustSharepointLink("https://educationgovuk.sharepoint.com")
            .withAdvisoryBoardDate("12", "09", "2023")
            .withAdvisoryBoardConditions("Test conditions")
            .withProvisionalTransferDate("13", "09", "2023")
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

    it.skip("Should be able to create a new Form a MAT transfer project", () => {
        // skip bug 202345
        homePage.addAProject();

        selectProjectTypePage.selectFormAMATTransfer().continue();

        newTransferPage
            .withAcademyURN(urnMAT)
            .withOutgoingTrustUKPRN("10058682")
            .withTrustReferenceNumber("TR04024")
            .withTrustName("Helix Academies Trust")
            .withAcademySharepointLink("https://educationgovuk.sharepoint.com")
            .withIncomingTrustSharePointLink("https://educationgovuk.sharepoint.com")
            .withOutgoingTrustSharepointLink("https://educationgovuk.sharepoint.com")
            .withAdvisoryBoardDate("26", "09", "2024")
            .withAdvisoryBoardConditions("Test conditions")
            .withProvisionalTransferDate("13", "09", "2023")
            .with2RI("Yes")
            .withTransferDueToInadequateOfstedRating("Yes")
            .withTransferDueToFinancialSafeguardingGovernanceIssues("Yes")
            .withOutgoingTrustWillCloseAfterTransfer("Yes")
            .withHandingOverToRCS("Yes")
            .continue()

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
            .withProvisionalTransferDate("19", "19", "2025")
            .continue();

        validationComponent
            .hasLinkedValidationError("The Urn must be 6 digits long. For example, 123456.")
            .hasLinkedValidationError("The OutgoingUKPRN must be 8 digits long and start with a 1. For example, 12345678.")
            .hasLinkedValidationError("The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234")
            .hasLinkedValidationError("The School or academy SharePoint link must have the https scheme")
            .hasLinkedValidationError("The Incoming trust SharePoint link must have the https scheme")
            .hasLinkedValidationError("Enter Outgoing trust SharePoint link in the correct format. SharePoint links start with 'https://educationgovuk.sharepoint.com' or 'https://educationgovuk-my.sharepoint.com/'")
            .hasLinkedValidationError("Advisory Board Date must be a real date")
            .hasLinkedValidationError("Provisional Transfer Date must be a real date")
            .hasLinkedValidationError("State if the conversion is due to 2RI. Choose yes or no")
            .hasLinkedValidationError("State if the transfer is due to an inadequate Ofsted rating. Choose yes or no")
            .hasLinkedValidationError("State if the transfer is due to financial, safeguarding or governance issues. Choose yes or no")
            .hasLinkedValidationError("State if the outgoing trust will close once this transfer is completed. Choose yes or no")
            .hasLinkedValidationError("State if this project will be handed over to the Regional casework services team. Choose yes or no");
    });
});
