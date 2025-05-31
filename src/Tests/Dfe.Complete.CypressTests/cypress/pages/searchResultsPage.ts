import Projects from "cypress/pages/projects/projects";

class SearchResultsPage extends Projects {
    hasSearchResultsTitle(searchTerm: string) {
        this.containsHeading(`Search results for "${searchTerm}"`);
        return this;
    }
}

const searchResultsPage = new SearchResultsPage();

export default searchResultsPage;
