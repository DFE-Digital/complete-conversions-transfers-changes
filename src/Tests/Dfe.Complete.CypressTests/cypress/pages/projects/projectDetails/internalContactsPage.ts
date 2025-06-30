import { ProjectDetailsPage } from "cypress/pages/projects/projectDetails/projectDetailsPage";

class InternalContactsPage extends ProjectDetailsPage {
    private readonly userInputId = "user-autocomplete";
    private readonly firstOptionId = "user-autocomplete__option--0";

    constructor() {
        super("projectInternalContacts");
    }

    hasEmailLink(email: string) {
        return this.hasTextWithLink("Email", `mailto:${email}`, 0);
    }

    hasChangeLink(link: string) {
        return this.hasTextWithLink("Change", link);
    }

    change(key: string) {
        cy.contains("dt", key).next("dd").next("dd").contains("Change").click();
        return this;
    }

    hasLabel(label: string) {
        cy.get(`label[for="${this.userInputId}"]`).should("contain.text", label);
        return this;
    }

    assignTo(user: string) {
        cy.getById(this.userInputId).clear().type(user);
        cy.getById(this.firstOptionId).click();
        return this;
    }

    selectTeam(team: string) {
        cy.contains(team).click();
        return this;
    }
}

const internalContactsPage = new InternalContactsPage();

export default internalContactsPage;
