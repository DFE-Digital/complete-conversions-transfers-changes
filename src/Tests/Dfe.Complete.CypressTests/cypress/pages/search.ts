class Search {
    private readonly searchId = "searchterm";
    private readonly searchButtonClass = "dfe-search__submit";

    searchFor(phrase: string) {
        cy.getById(this.searchId).type(phrase);
        cy.getByClass(this.searchButtonClass).click();
    }
}

const search = new Search();

export default search;
