class Projects {
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

    doesNotContainFilter(filter: string) {
        cy.getByClass("moj-primary-navigation__list").should("not.contain", filter);
        return this;
    }
}

export default Projects;
