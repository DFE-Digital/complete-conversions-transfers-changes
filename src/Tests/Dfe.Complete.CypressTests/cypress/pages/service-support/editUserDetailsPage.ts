import BasePage from "cypress/pages/basePage";

class EditUserDetailsPage extends BasePage {
    hasFirstName(firstName: string) {
        cy.getById("FirstName").should("have.value", firstName);
        return this;
    }

    hasLastName(lastName: string) {
        cy.getById("LastName").should("have.value", lastName);
        return this;
    }
    hasEmail(email: string) {
        cy.getById("Email").should("have.value", email);
        return this;
    }

    hasTeam(team: string) {
        cy.getByRadioOption(team).should("be.checked");
        return this;
    }

    withFirstName(firstName: string) {
        cy.getById("FirstName").clear().typeFast(firstName);
        return this;
    }

    withLastName(lastName: string) {
        cy.getById("LastName").clear().typeFast(lastName);
        return this;
    }

    withEmail(email: string) {
        cy.getById("Email").clear().typeFast(email);
        return this;
    }

    withTeam(team: string) {
        cy.getByRadioOption(team).check();
        return this;
    }

    saveAndReturn() {
        this.clickButton("Save and return");
        return this;
    }
}

const editUserDetailsPage = new EditUserDetailsPage();

export default editUserDetailsPage;
