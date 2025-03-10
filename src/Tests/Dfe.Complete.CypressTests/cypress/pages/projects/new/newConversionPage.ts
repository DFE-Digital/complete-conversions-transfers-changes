import NewProjectPage from "./newProjectPage";

class NewConversionPage extends NewProjectPage {

    public selectConversion(): this {
        cy.getById("ProjectType").click();

        return this;
    }

    public withSchoolURN(urn: string): this {
        cy.getById("URN").typeFast(urn);
        return this;
    }

    public withIncomingTrustUKPRN (ukprn: string): this {
        cy.getById("UKPRN").typeFast(ukprn);
        return this;
    }

    public withProvisionalConversionDate(day: string, month: string, year: string): this {
        cy.enterDate("SignificantDate", day, month, year);

        return this;
    }

    public withSchoolSharepointLink(link: string): this {
        cy.getById("SchoolSharePointLink").typeFast(link);
        return this;
    }

    public withAcademyOrder(option: "Directive academy order" | "Academy order"): this {
        if (option == "Directive academy order")
            cy.getById("DirectiveAcademyOrder").click();
        if (option == "Academy order")
            cy.getById("DirectiveAcademyOrder-2").click();
        return this;
    }
}

const newConversionPage = new NewConversionPage();

export default newConversionPage;