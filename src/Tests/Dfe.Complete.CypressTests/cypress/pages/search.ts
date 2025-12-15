class Search {
    private readonly searchToggleId = "search-toggle-button";
    private readonly searchInputId = "search-overlay-input";
    private readonly searchButtonId = "search-submit";

    clickSearch() {
        cy.getById(this.searchToggleId).click();
        return this;
    }

    searchFor(phrase: string) {
        cy.getById(this.searchInputId).type(phrase);
        cy.getById(this.searchButtonId).click();
        return this;
    }
}

const search = new Search();

export default search;
