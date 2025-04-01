class ProjectDetailsPage {
    containsHeading(heading: string) {
        cy.get('h1').should('contain', heading);
        return this;
    }


}

const projectDetailsPage = new ProjectDetailsPage();

export default projectDetailsPage;