class BasePage {
    private readonly bannerClass = "govuk-notification-banner";

    containsHeading(heading: string) {
        cy.get("h1").contains(heading);
        return this;
    }

    containsSubHeading(subHeading: string) {
        cy.get("h2").contains(subHeading);
        return this;
    }

    containsImportantBannerWithMessage(title: string, message: string) {
        return this.containsBannerWithMessage("Important", title, message);
    }

    clickButton(buttonText?: string) {
        if (buttonText) {
            cy.getByClass("govuk-button").contains(buttonText).click();
        } else {
            cy.getByClass("govuk-button").click();
        }
        return this;
    }

    verifyFieldDoesntExistOnAnyPage(field: string) {
        cy.get("body").then(($body) => {
            if ($body.find(`.govuk-table:contains(${field})`).length > 0) {
                throw new Error(`Field "${field}" exists on the current page.`);
            }
            if ($body.find(`#next-page`).length > 0) {
                this.goToNextPage();
                this.verifyFieldDoesntExistOnAnyPage(field);
            }
        });
        return this;
    }

    goToNextPageUntilFieldIsVisible(field: string) {
        cy.get("body").then(($body) => {
            if ($body.find(`.govuk-table:contains(${field})`).length === 0) {
                this.goToNextPage();
                this.goToNextPageUntilFieldIsVisible(field);
            }
        });
        return this;
    }

    goToPreviousPageUntilFieldIsVisible(field: string) {
        cy.get("body").then(($body) => {
            if ($body.find(`.govuk-table:contains(${field})`).length === 0) {
                this.goToPreviousPage();
                this.goToPreviousPageUntilFieldIsVisible(field);
            }
        });
        return this;
    }

    goToNextPage() {
        cy.getById("next-page").click();
        return this;
    }

    goToPreviousPage() {
        cy.getById("previous-page").click();
        return this;
    }

    goToLastPage() {
        cy.getByClass("govuk-pagination__list").find("li").last().click();
        return this;
    }

    private containsBannerWithMessage(bannerType: string, title: string, message: string) {
        cy.getByClass(this.bannerClass).within(() => {
            cy.get("h2").should("have.text", bannerType);
            cy.get("h3").should("have.text", title);
            cy.get("p").should("have.text", message);
        });
        return this;
    }
}

export default BasePage;
