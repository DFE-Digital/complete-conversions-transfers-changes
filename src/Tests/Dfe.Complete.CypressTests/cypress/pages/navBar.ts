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

    goToGroups() {
        cy.getById(this.navHeaderId).contains("Groups").click();
        return this;
    }

    goToServiceSupport() {
        cy.getById(this.navHeaderId).contains("Service support").click();
        return this;
    }

    unableToViewYourProjects() {
        cy.getById(this.navHeaderId).contains("Your projects").should("not.exist");
        return this;
    }

    unableToViewYourTeamProjects() {
        cy.getById(this.navHeaderId).contains("Your team projects").should("not.exist");
        return this;
    }

    unableToViewGroups() {
        cy.getById(this.navHeaderId).contains("Groups").should("not.exist");
        return this;
    }

    unableToViewServiceSupport() {
        cy.getById(this.navHeaderId).contains("Service support").should("not.exist");
        return this;
    }
}

const navBar = new NavBar();

export default navBar;
