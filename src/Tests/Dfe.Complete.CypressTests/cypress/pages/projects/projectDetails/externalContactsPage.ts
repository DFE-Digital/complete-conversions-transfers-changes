import { DetailsPage } from "cypress/pages/detailsPage";

class ExternalContactsPage extends DetailsPage {
    setContactItem(index: number) {
        return this.setSectionCounter(index - 1);
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
}

const externalContactsPage = new ExternalContactsPage();

export default externalContactsPage;
