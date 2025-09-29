import BasePage from "cypress/pages/basePage";

export class DetailsPage extends BasePage {
    protected readonly sectionItemClass = "govuk-summary-card";

    inOrder() {
        cy.wrap(-1).as("sectionCounter");
        return this;
    }

    setSectionCounter(index: number) {
        cy.wrap(index).as("sectionCounter");
        return this;
    }

    hasSectionItem() {
        cy.wrap(-1).as("summaryCounter");
        cy.wrap(-1).as("reasonCounter");
        cy.get("@sectionCounter").then((counter) => {
            const nextIndex = Number(counter) + 1;
            cy.wrap(nextIndex).as("sectionCounter");
            cy.getByClass(this.sectionItemClass).eq(nextIndex).should("exist");
        });
        return this;
    }

    hasSubHeading(subHeading: string) {
        cy.get("@sectionCounter").then((index) => {
            cy.getByClass(this.sectionItemClass).eq(Number(index)).find("h2").contains(subHeading);
        });
        return this;
    }

    summaryShows(key: string): this {
        cy.get("@sectionCounter").then((sectionItem) => {
            cy.get("@summaryCounter").then((counter) => {
                const nextIndex = Number(counter) + 1;
                cy.wrap(nextIndex).as("summaryCounter");
                cy.getByClass(this.sectionItemClass)
                    .eq(Number(sectionItem))
                    .find(".govuk-summary-list__key")
                    .eq(nextIndex)
                    .shouldHaveText(key);
            });
        });
        return this;
    }

    hasValue(value: string | number) {
        cy.get("@sectionCounter").then((sectionItem) => {
            cy.get("@summaryCounter").then((counter) => {
                cy.getByClass(this.sectionItemClass)
                    .eq(Number(sectionItem))
                    .find(".govuk-summary-list__value")
                    .eq(Number(counter))
                    .shouldHaveText(value);
            });
        });
        return this;
    }
}

const detailsPage = new DetailsPage();

export default detailsPage;
