import BasePage from "cypress/pages/basePage";

class Projects extends BasePage {
    private readonly subNavClass = "moj-primary-navigation__list";

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
