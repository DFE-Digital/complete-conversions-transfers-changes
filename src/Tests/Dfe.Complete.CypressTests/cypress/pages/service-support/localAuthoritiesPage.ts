import BasePage from "cypress/pages/basePage";

class LocalAuthoritiesPage extends BasePage {
    newLocalAuthority() {
        cy.getByClass("govuk-button").contains("New local authority").click();
        return this;
    }

    viewLocalAuthorityDetails(name: string) {
        cy.contains(name).parents("tr").contains("View details").click();
        return this;
    }

    authorityDeletedSuccessMessage() {
        cy.getByClass("govuk-notification-banner--success").contains("Local authority deleted successfully");
        return this;
    }

    localAuthorityDoesNotExistAcrossAllPages(name: string) {
        this.verifyFieldDoesntExistOnAnyPage(name);
        return this;
    }
}

const localAuthoritiesPage = new LocalAuthoritiesPage();

export default localAuthoritiesPage;
