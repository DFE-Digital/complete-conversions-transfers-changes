import Projects from "./projects";

class AllProjects extends Projects {

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

