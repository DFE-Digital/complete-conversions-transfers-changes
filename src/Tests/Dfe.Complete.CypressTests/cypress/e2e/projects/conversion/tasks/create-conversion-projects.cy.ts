
import projectRemover from "cypress/api/projectRemover";
import newConversionPage from "cypress/pages/projects/new/newConversionPage";
import selectProjectType from "cypress/pages/projects/new/selectProjectTypePage";

const urn : string = "401450";

describe("Create a new Conversion Project", () => {

    beforeEach(() => {
        projectRemover.removeProject(urn);
        cy.login({role: "RegionalDeliveryOfficer"});
    });

    it("Should be able to move around the complete service", () => {
        cy.visit(`/`);

        cy.getByClass("govuk-button").click()
        
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
            .Continue()
    });
});
