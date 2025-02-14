import projectRemover from "cypress/api/projectRemover";
import newConversionPage from "cypress/pages/projects/new/newConversionPage";
import homePage from "cypress/pages/homePage";
import selectProjectType from "cypress/pages/projects/new/selectProjectTypePage";
import validationComponent from "cypress/pages/validationComponent";

const urn: string = "401450";

describe("Create a new Conversion Project", () => {
    beforeEach(() => {
        projectRemover.removeProjectIfItExists(urn);
        cy.login({ role: "RegionalDeliveryOfficer" });
    });

    it("Should be able to move around the complete service", () => {
        cy.visit(`/`);

        homePage.addAProject();

        //cy.executeAccessibilityTests();

        selectProjectType.selectConversion()
        .continue()

        newConversionPage
            .WithSchoolURN(urn)
            .WithIncomingTrustUKPRN("10059853")
            .withAdvisoryBoardDate("10", "12", "2025")
            .withProvisionalConversionDate("9", "11", "2026")
            .WithSchoolSharepointLink("https://educationgovuk-my.sharepoint.com/")
            .WithIncomingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/")
            .WithHandingOverToRCS("No")
            .WithAcademyOrder("Academy order")
            .With2RI("No")
            .Continue();
    });

    it("Should show multiple validation errors when continuing with no input", () => {
        cy.visit(`/`);

        homePage.addAProject();

        //cy.executeAccessibilityTests();

        selectProjectType.selectConversion().continue();

        newConversionPage.Continue()

        validationComponent
            .hasLinkedValidationError("The Urn field is required")
            .hasLinkedValidationError("Enter a UKPRN")
            .hasLinkedValidationError("Enter a date for the Advisory Board Date, like 1 4 2023")
            .hasLinkedValidationError("Enter a date for the Provisional Conversion Date, like 1 4 2023")
            .hasLinkedValidationError("The School or academy SharePoint link field is required.")
            .hasLinkedValidationError("The Incoming trust SharePoint link field is required.")
            .hasLinkedValidationError("State if this project will be handed over to the Regional casework services team. Choose yes or no")
            .hasLinkedValidationError("Select directive academy order or academy order, whichever has been used for this conversion")
            .hasLinkedValidationError("State if the conversion is due to 2RI. Choose yes or no")
    });
});
