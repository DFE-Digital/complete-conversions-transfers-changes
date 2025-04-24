import Projects from "./projects";

class YourTeamProjects extends Projects {
    noProjectsShown() {
        cy.contains("There are no projects in progress.");
        return this;
    }
}

const yourTeamProjects = new YourTeamProjects();

export default yourTeamProjects;
