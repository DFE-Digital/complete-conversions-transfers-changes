import BasePage from "cypress/pages/basePage";

class DateHistoryPage extends BasePage {
    private readonly dateHistoryItemClass = "govuk-summary-card";

    inOrder() {
        cy.wrap(-1).as("dateHistoryCounter");
        return this;
    }

    hasDateHistoryItem() {
        cy.wrap(-1).as("summaryCounter");
        cy.wrap(-1).as("reasonCounter");
        cy.get("@dateHistoryCounter").then((counter) => {
            const nextIndex = Number(counter) + 1;
            cy.wrap(nextIndex).as("dateHistoryCounter");
            cy.getByClass(this.dateHistoryItemClass).eq(nextIndex).should("exist");
        });
        return this;
    }

    hasSubHeading(subHeading: string) {
        cy.get("@dateHistoryCounter").then((index) => {
            cy.getByClass(this.dateHistoryItemClass).eq(Number(index)).find("h2").contains(subHeading);
        });
        return this;
    }

    summaryShows(key: string): this {
        cy.get("@dateHistoryCounter").then((dateHistoryItem) => {
            cy.get("@summaryCounter").then((counter) => {
                const nextIndex = Number(counter) + 1;
                cy.wrap(nextIndex).as("summaryCounter");
                cy.getByClass(this.dateHistoryItemClass)
                    .eq(Number(dateHistoryItem))
                    .find(".govuk-summary-list__key")
                    .eq(nextIndex)
                    .shouldHaveText(key);
            });
        });
        return this;
    }

    hasValue(value: string | number) {
        cy.get("@dateHistoryCounter").then((dateHistoryItem) => {
            cy.get("@summaryCounter").then((counter) => {
                cy.getByClass(this.dateHistoryItemClass)
                    .eq(Number(dateHistoryItem))
                    .find(".govuk-summary-list__value")
                    .eq(Number(counter))
                    .shouldHaveText(value);
            });
        });
        return this;
    }

    hasReasonNewDate(category: string, reason: string) {
        cy.get("@dateHistoryCounter").then((dateHistoryItem) => {
            cy.get("@summaryCounter").then((counter) => {
                cy.get("@reasonCounter").then((reasonCounter) => {
                    const nextIndex = Number(reasonCounter) + 1;
                    cy.wrap(nextIndex).as("reasonCounter");
                    cy.getByClass(this.dateHistoryItemClass)
                        .eq(Number(dateHistoryItem))
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
