import { DetailsPage } from "cypress/pages/detailsPage";

class DateHistoryPage extends DetailsPage {
    hasDateHistoryItem() {
        return this.hasSectionItem();
    }
    hasReasonNewDate(category: string, reason: string) {
        cy.get("@sectionCounter").then((sectionItem) => {
            cy.get("@summaryCounter").then((counter) => {
                cy.get("@reasonCounter").then((reasonCounter) => {
                    const nextIndex = Number(reasonCounter) + 1;
                    cy.wrap(nextIndex).as("reasonCounter");
                    cy.getByClass(this.sectionItemClass)
                        .eq(Number(sectionItem))
                        .find(".govuk-summary-list__value")
                        .eq(Number(counter))
                        .find("li")
                        .eq(Number(nextIndex))
                        .shouldHaveText(`${category} ${reason}`);
                });
            });
        });
        return this;
    }
}

const dateHistoryPage = new DateHistoryPage();

export default dateHistoryPage;
