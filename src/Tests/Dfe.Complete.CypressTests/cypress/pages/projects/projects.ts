class Projects {
    private readonly subNavClass = "moj-primary-navigation__list";

    containsHeading(heading: string) {
        cy.get("h1").contains(heading);
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

    filterProjects(filter: string) {
        cy.getByClass("moj-primary-navigation__list").contains(filter).click();
        return this;
    }

    ableToViewFilters(filters: string[]) {
        this.expectToView(filters, true);
        return this;
    }

    unableToViewFilter(filter: string) {
        this.expectToView([filter], false);
        return this;
    }

    private expectToView(filters: string[], visible: boolean) {
        filters.forEach((filter) => {
            cy.getByClass(this.subNavClass)
                .contains(filter)
                .should(visible ? "exist" : "not.exist");
        });
        return this;
    }
}

export default Projects;
