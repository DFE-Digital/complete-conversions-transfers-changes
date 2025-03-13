class AllProjects {

    containsHeading(heading: string) {
        cy.get('h1').contains(heading);
        return this;
    }

    filterProjects(filter: string) {
        cy.getByClass('moj-primary-navigation__list').contains(filter).click();
        return this;
    }

    viewConversionsProjects(){
        cy.get('nav').contains('Conversions').click();
        return this;
    }

    viewTransfersProjects(){
        cy.get('nav').contains('Transfers').click();
        return this;
    }

    goToNextPageUntilFieldIsVisible(field: string) {
        cy.get('body').then($body => {
            if ($body.find(`.govuk-table:contains(${field})`).length === 0) {
                this.goToNextPage();
                this.goToNextPageUntilFieldIsVisible(field);
            }
        });
        return this;
    }

    goToNextPage(){
        cy.getById('next-page').click();
        return this;
    }
}
const allProjects = new AllProjects();

export default allProjects;

