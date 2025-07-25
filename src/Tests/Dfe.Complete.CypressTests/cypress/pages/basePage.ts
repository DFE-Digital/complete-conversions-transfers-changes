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

    contains(text: string) {
        cy.contains(text);
        return this;
    }

    doesntContain(text: string) {
        cy.contains(text).should("not.exist");
        return this;
    }

    containsImportantBannerWithMessage(title: string, message?: string) {
        return this.containsBannerWithMessage("Important", title, message);
    }

    containsSuccessBannerWithMessage(title: string, message?: string) {
        return this.containsBannerWithMessage("Success", title, message);
    }

    clickButton(buttonText?: string) {
        if (buttonText) {
            cy.getByClass("govuk-button").contains(buttonText).click();
        } else {
            cy.getByClass("govuk-button").click();
        }
        return this;
    }

    hasButton(buttonText: string) {
        cy.getByClass("govuk-button").contains(buttonText).should("be.visible").should("not.be.disabled");
        return this;
    }

    notAuthorisedToPerformThisActionBanner() {
        this.containsImportantBannerWithMessage("You are not authorised to perform this action.");
        return this;
    }

    clickLink(linkText: string) {
        cy.getByClass("govuk-link").contains(linkText).click();
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
        cy.get("body").then(($body) => {
            if ($body.find(".govuk-pagination__list").length > 0) {
                cy.getByClass("govuk-pagination__list").find("li").last().click();
            }
        });
        return this;
    }

    jumpToSection(section: string) {
        cy.contains("Jump to section")
            .parents("nav")
            .within(() => {
                cy.contains(section).click();
            });
        return this;
    }

    protected pageHasMovedToSection(section: string, sections: Record<string, string>) {
        cy.url().should("include", `#${sections[section]}`);
        cy.contains("h2", section).isInViewport();
        return this;
    }

    private containsBannerWithMessage(bannerType: string, title: string, message?: string) {
        cy.getByClass(this.bannerClass)
            .first()
            .within(() => {
                cy.get("h2").should("contain.text", bannerType);
                cy.get("h3").shouldHaveText(title);
                if (message) {
                    cy.get("p").shouldHaveText(message);
                }
            });
        return this;
    }
}

export default BasePage;
