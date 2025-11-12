class Search {
    private readonly searchId = "searchterm";
    private readonly searchButtonId = "search-submit";

    searchFor(phrase: string) {
        cy.getById(this.searchId).type(phrase);
        cy.getById(this.searchButtonId).click();
    }
}

const search = new Search();

export default search;
