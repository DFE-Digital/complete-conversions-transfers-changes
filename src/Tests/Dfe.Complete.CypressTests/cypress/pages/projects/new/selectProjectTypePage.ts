class SelectProjectTypePage {
    public selectConversion(): this {
        cy.getById("ProjectType").click();

        return this;
    }

    public continue(): this {
        cy.getByClass("govuk-button").click();
        return this;
    }
}

const selectProjectTypePage = new SelectProjectTypePage();

export default selectProjectTypePage;