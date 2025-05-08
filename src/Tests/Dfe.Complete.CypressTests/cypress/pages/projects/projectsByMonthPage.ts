class ProjectsByMonthPage {
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
        cy.getByClass("govuk-button").contains("Apply").click();
        return this;
    }

    filterDoesNotExist() {
        return this;
    }

    viewConversionProjects() {
        cy.contains("Conversions").click(); // add id to button
        return this;
    }

    viewTransferProjects() {
        cy.contains("Transfers").click(); // add id to button
        return this;
    }
}

const projectsByMonthPage = new ProjectsByMonthPage();

export default projectsByMonthPage;
