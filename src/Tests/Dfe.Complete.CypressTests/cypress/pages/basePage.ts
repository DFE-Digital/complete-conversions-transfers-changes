class BasePage {
    protected readonly bannerClass = "govuk-notification-banner";
    private readonly linkClass = "govuk-link";

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

    doesntContain(text: string | RegExp) {
        cy.contains(text).should("not.exist");
        return this;
    }

    doesntContainButton(buttonText: string | RegExp) {
        cy.contains("button", buttonText).should("not.exist");
        cy.contains("a", buttonText).should("not.exist");
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

    type(text: string, fieldId?: string) {
        if (fieldId) {
            cy.getById(fieldId).clear().typeFast(text);
        } else {
            cy.get("textarea").first().clear().typeFast(text);
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
        cy.getByClass(this.linkClass).contains(linkText).click();
        return this;
    }

    linkDoesNotExist(linkText: string) {
        cy.contains("a", linkText).should("not.exist");
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
        cy.contains("h2", section).should("be.visible");
        return this;
    }

    private containsBannerWithMessage(bannerType: string, title: string, message?: string) {
        let foundMatch = false;
        cy.getByClass(this.bannerClass)
            .should("exist")
            .each(($banner) => {
                const hasCorrectType = $banner.find("h2").text().includes(bannerType);
                const hasCorrectTitle = title ? $banner.find("h3").text().includes(title) : true;
                const hasCorrectMessage = message ? $banner.find("p").text().includes(message) : true;

                if (hasCorrectType && hasCorrectTitle && hasCorrectMessage) {
                    foundMatch = true;
                }
            })
            .then(() => {
                expect(
                    foundMatch,
                    `Expected to find banner with type "${bannerType}", title "${title}"${
                        message ? `, and message "${message}"` : ""
                    }`,
                ).to.be.true;
            });
        return this;
    }
}

export default BasePage;
