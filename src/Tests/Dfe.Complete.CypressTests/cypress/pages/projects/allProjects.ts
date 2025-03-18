import Projects from "./projects";

class AllProjects extends Projects {

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

}
const allProjects = new AllProjects();

export default allProjects;

