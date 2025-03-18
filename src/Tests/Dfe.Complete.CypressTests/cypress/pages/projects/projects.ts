class Projects {

    containsHeading(heading: string) {
        cy.get('h1').contains(heading);
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

export default Projects;