class DateRangeFilter {

    selectDateFrom(date: string) {
        cy.getById("from_date").select(date);
        cy.getById("from_date").should("have.value", date);
        return this;
    }

    selectDateTo(date: string) {
        cy.getById("to_date").select(date);
        cy.getById("to_date").should("have.value", date);
        return this;
    }

    applyDateFilter() {
        cy.getByClass("govuk-button").click();
        return this;
    }

}

const dateRangeFilter = new DateRangeFilter();

export default dateRangeFilter;