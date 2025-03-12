import projectRemover from "cypress/api/projectRemover";
import newConversionPage from "cypress/pages/projects/new/newConversionPage";
import homePage from "cypress/pages/homePage";
import selectProjectType from "cypress/pages/projects/new/selectProjectTypePage";
import validationComponent from "cypress/pages/validationComponent";

const urn: string = "111394";
const urnMAT: string = "103846"; //103842

describe("Create a new Conversion Project", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(urn);
        // projectRemover.removeProjectIfItExists(urnMAT); // skip bug 202345
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should be able to create a new conversion project", () => {
        homePage.addAProject();

        // cy.executeAccessibilityTests();

        selectProjectType.selectConversion().continue();

        newConversionPage
            .withSchoolURN(urn)
            .withIncomingTrustUKPRN("10059853")
            .withAdvisoryBoardDate("10", "12", "2024")
            .withProvisionalConversionDate("9", "11", "2026")
            .withSchoolSharepointLink("https://educationgovuk-my.sharepoint.com/")
            .withIncomingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/")
            .withHandingOverToRCS("No")
            .withAcademyOrder("Academy order")
            .with2RI("No")
            .continue();

        validationComponent.hasNoValidationErrors()
        cy.get("h2").should("contain", "Project created");
    });

    it.skip("Should be able to create a new Form a MAT conversion project", () => {
        // skip bug 202345
        homePage.addAProject();

        selectProjectType.selectFormAMATConversion().continue();

        newConversionPage
            .withSchoolURN(urnMAT)
            .withTrustReferenceNumber("TR09999")
            .withTrustName("Test Trust")
            .withAdvisoryBoardDate("12", "12", "2024")
            .withAdvisoryBoardConditions("Test conditions")
            .withProvisionalConversionDate("25", "11", "2026")
            .withSchoolSharepointLink("https://educationgovuk-my.sharepoint.com/")
            .withIncomingTrustSharePointLink("https://educationgovuk-my.sharepoint.com/")
            .withHandingOverToRCS("Yes")
            .withHandoverComments("Test comments")
            .withAcademyOrder("Directive academy order")
            .with2RI("Yes")
            .continue();

        validationComponent.hasNoValidationErrors();
        cy.get("h2").should("contain", "Project created");
    });

    it("Should show multiple validation errors when continuing with no input", () => {
        homePage.addAProject();

        //cy.executeAccessibilityTests();

        selectProjectType.selectConversion().continue();

        newConversionPage.continue();

        validationComponent
            .hasLinkedValidationError("The Urn field is required")
            .hasLinkedValidationError("Enter a UKPRN")
            .hasLinkedValidationError("Enter a date for the Advisory Board Date, like 1 4 2023")
            .hasLinkedValidationError("Provisional Conversion Date must include a month and year")
            .hasLinkedValidationError("The School or academy SharePoint link field is required.")
            .hasLinkedValidationError("The Incoming trust SharePoint link field is required.")
            .hasLinkedValidationError(
                "State if this project will be handed over to the Regional casework services team. Choose yes or no",
            )
            .hasLinkedValidationError(
                "Select directive academy order or academy order, whichever has been used for this conversion",
            )
            .hasLinkedValidationError("State if the conversion is due to 2RI. Choose yes or no");
    });
});
