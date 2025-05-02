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

    goToNextPage() {
        cy.getById("next-page").click();
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
