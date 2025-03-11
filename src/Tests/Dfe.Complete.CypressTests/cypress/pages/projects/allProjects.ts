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

    goToNextPageUntilProjectIsVisible(schoolName: string) {
        cy.get('body').then($body => {
            if ($body.find(`.govuk-table:contains(${schoolName})`).length === 0) {
                this.goToNextPage();
                this.goToNextPageUntilProjectIsVisible(schoolName);
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

