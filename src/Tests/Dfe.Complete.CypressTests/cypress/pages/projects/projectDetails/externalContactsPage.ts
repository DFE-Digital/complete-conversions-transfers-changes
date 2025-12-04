import { DetailsPage } from "cypress/pages/detailsPage";

class ExternalContactsPage extends DetailsPage {
    setContactItem(index: number) {
        return this.setSectionCounter(index - 1);
    }

    setContactItemByRoleHeading(role: string) {
        cy.getByClass(this.sectionItemClass).each(($el, index) => {
            cy.wrap($el)
                .find("h3")
                .then(($h3) => {
                    if ($h3.text().trim() === role) {
                        cy.wrap(index - 1).as("sectionCounter");
                        return false; // break
                    }
                });
        });
        return this;
    }

    containsOrganisationHeading(subHeading: string) {
        cy.get("h3").contains(subHeading);
        return this;
    }

    hasContactItem() {
        return this.hasSectionItem();
    }

    hasRoleHeading(role: string) {
        cy.get("@sectionCounter").then((index) => {
            cy.getByClass(this.sectionItemClass).eq(Number(index)).find("h3").contains(role);
        });
        return this;
    }

    editContact() {
        cy.get("@sectionCounter").then((index) => {
            cy.getByClass(this.sectionItemClass)
                .eq(Number(index))
                .within(() => {
                    cy.contains("a", "Edit contact").click();
                });
        });
    }
}

const externalContactsPage = new ExternalContactsPage();

export default externalContactsPage;
