import { EditUserPage } from "cypress/pages/projects/editUserPage";

class InternalContactsPage extends EditUserPage {
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

    hasNoChangeLink(key: string) {
        cy.contains("dt", key).next("dd").next("dd").should("not.contain.text", "Change");
        return this;
    }

    selectTeam(team: string) {
        cy.contains(team).click();
        return this;
    }
}

const internalContactsPage = new InternalContactsPage();

export default internalContactsPage;
