class Search {
    private searchId = "searchterm";
    private searchButtonClass = "dfe-search__submit";

    searchFor(phrase: string) {
        cy.getById(this.searchId).type(phrase);
        cy.getByClass(this.searchButtonClass).click();
    }
}

const search = new Search();

export default search;
