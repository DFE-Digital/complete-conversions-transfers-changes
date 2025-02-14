class NewConversionPage {
    public selectConversion(): this {
        cy.getById("ProjectType").click();

        return this;
    }

    public WithSchoolURN(urn: string): this {
        cy.getById("URN").typeFast(urn);
        return this;
    }

    
    public WithIncomingTrustUKPRN (ukprn: string): this {
        cy.getById("UKPRN").typeFast(ukprn);
        return this;
    }
    
    public WithGroupReferenceNumber(grp: string): this {
        cy.getById("GroupReferenceNumber").typeFast(grp);
        return this;
    }

    public withAdvisoryBoardDate(day: string, month: string, year: string): this {
        cy.enterDate("AdvisoryBoardDate", day, month, year);

        return this;
    }


    public withProvisionalConversionDate(day: string, month: string, year: string): this {
        cy.enterDate("ProvisionalConversionDate", day, month, year);

        return this;
    }

    public WithAdvisoryBoardConditions(text: string): this {
        cy.getById("AdvisoryBoardConditions").typeFast(text);
        return this;
    }

    public WithSchoolSharepointLink(link: string): this {
        cy.getById("SchoolSharePointLink").typeFast(link);
        return this;
    }

    public WithIncomingTrustSharePointLink(link: string): this {
        cy.getById("IncomingTrustSharePointLink").typeFast(link);
        return this;
    }

    public WithHandingOverToRCS(option: "Yes" | "No"): this {
        if (option == "Yes")
            cy.getById("IsHandingToRCS").click();
        if (option == "No")
            cy.getById("IsHandingToRCS-2").click();
        return this;
    }
    
    public WithHandoverComments(text: string): this {
        cy.getById("HandoverComments").typeFast(text);
        return this;
    }

    public WithAcademyOrder(option: "Directive academy order" | "Academy order"): this {
        if (option == "Directive academy order")
            cy.getById("DirectiveAcademyOrder").click();
        if (option == "Academy order")
            cy.getById("DirectiveAcademyOrder-2").click();
        return this;
    }

    
    public With2RI(option: "Yes" | "No"): this {
        if (option == "Yes")
            cy.getById("IsDueTo2RI").click();
        if (option == "No")
            cy.getById("IsDueTo2RI-2").click();
        return this;
    }

    public Continue(): this {
        cy.getByClass("govuk-button").click();
        return this;
    }
}

const newConversionPage = new NewConversionPage();

export default newConversionPage;