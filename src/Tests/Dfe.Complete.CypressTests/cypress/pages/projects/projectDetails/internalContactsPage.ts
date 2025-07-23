import { EditUserPage } from "cypress/pages/projects/editUserPage";

class InternalContactsPage extends EditUserPage {
    private readonly sectionId = "projectInternalContacts";

    inOrder() {
        cy.wrap(this.sectionId).as("sectionId");
        cy.wrap(-1).as("summaryCounter");
        return this;
    }

    row(row: number) {
        cy.wrap(this.sectionId).as("sectionId");
        cy.wrap(row - 2).as("summaryCounter");
        return this;
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
