import { ProjectDetailsPage } from "cypress/pages/projects/projectDetails/projectDetailsPage";

export class EditUserPage extends ProjectDetailsPage {
    private readonly userInputId = "user-autocomplete";
    private readonly userOptionsId = "user-autocomplete__listbox";
    private readonly firstOptionId = "user-autocomplete__option--0";

    hasLabel(label: string) {
        cy.get(`label[for="${this.userInputId}"]`).should("contain.text", label);
        return this;
    }

    assignTo(user: string) {
        cy.getById(this.userInputId).clear().type(user);
        cy.getById(this.userOptionsId).within(() => {
            cy.get("li").should("have.length", 1);
        });
        cy.getById(this.firstOptionId).click();
        return this;
    }

    assignToInvalidUser(user: string) {
        cy.getById(this.userInputId).clear().type(user);
        cy.getById(this.firstOptionId).should("not.exist");
        return this;
    }
}

const editUserPage = new EditUserPage();

export default editUserPage;
