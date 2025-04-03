class NavBar {
    private readonly navHeaderId = "header-navigation";

    goToAllProjects() {
        cy.getById(this.navHeaderId).contains("All projects").click();
        return this;
    }

    goToYourProjects() {
        cy.getById(this.navHeaderId).contains("Your projects").click();
        return this;
    }

    goToYourTeamProjects() {
        cy.getById(this.navHeaderId).contains("Your team projects").click();
        return this;
    }
}

const navBar = new NavBar();

export default navBar;
