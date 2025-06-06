import BasePage from "cypress/pages/basePage";

class SelectProjectTypePage extends BasePage {
    public selectConversion(): this {
        cy.getById("ProjectType").click();
        return this;
    }

    public selectTransfer(): this {
        cy.getById("ProjectType-2").click();
        return this;
    }

    public selectFormAMATConversion(): this {
        cy.getById("ProjectType-3").click();
        return this;
    }

    public selectFormAMATTransfer(): this {
        cy.getById("ProjectType-4").click();
        return this;
    }

    public continue(): this {
        this.clickButton("Continue");
        return this;
    }
}

const selectProjectTypePage = new SelectProjectTypePage();

export default selectProjectTypePage;
