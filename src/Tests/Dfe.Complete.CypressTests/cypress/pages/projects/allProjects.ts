import Projects from "./projects";

class AllProjects extends Projects {
    private readonly projectsNavigation: string = "nav";

    viewConversionsProjects() {
        cy.get(this.projectsNavigation).contains("Conversions").click();
        return this;
    }

    viewTransfersProjects() {
        cy.get(this.projectsNavigation).contains("Transfers").click();
        return this;
    }

    viewFormAMatProjects() {
        cy.get(this.projectsNavigation).contains("Form a MAT").click();
        return this;
    }
}

const allProjects = new AllProjects();

export default allProjects;
