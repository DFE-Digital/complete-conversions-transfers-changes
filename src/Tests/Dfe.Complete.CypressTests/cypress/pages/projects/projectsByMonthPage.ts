import BasePage from "cypress/pages/basePage";

class ProjectsByMonthPage extends BasePage {
    private readonly fromDateId = "fromDate";
    private readonly toDateId = "toDate";

    filterIsFromDateToDate(expectedFromDate: string, expectedToDate: string) {
        cy.getById(this.fromDateId).find("option:selected").should("have.text", expectedFromDate);
        cy.getById(this.toDateId).find("option:selected").should("have.text", expectedToDate);
        return this;
    }

    filterDateRange(fromDate: string, toDate: string) {
        cy.getById(this.fromDateId).select(fromDate);
        cy.getById(this.toDateId).select(toDate);
        this.clickButton("Apply");
        return this;
    }

    filterDoesNotExist() {
        cy.getById(this.fromDateId).should("not.exist");
        cy.getById(this.toDateId).should("not.exist");
        return this;
    }

    viewConversionProjects() {
        cy.getByDataCy("conversions-tab").click();
        return this;
    }

    viewTransferProjects() {
        cy.getByDataCy("transfers-tab").click();
        return this;
    }
}

const projectsByMonthPage = new ProjectsByMonthPage();

export default projectsByMonthPage;
