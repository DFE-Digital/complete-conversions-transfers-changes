class DeleteLocalAuthorityPage {
    hasAreYouSureYouWantToDeleteMessage(name: string) {
        cy.get("h1").should("contain", `Are you sure you want to delete the record for ${name}?`);
        cy.contains(`This action will remove the local authority record for ${name} from this application.`);
        return this;
    }

    confirmDelete() {
        cy.contains("button.govuk-button", "Delete").click();
        return this;
    }
}

const deleteLocalAuthorityPage = new DeleteLocalAuthorityPage();

export default deleteLocalAuthorityPage;
