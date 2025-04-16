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

    ableToView(sections: string[]) {
        sections.forEach((section) => {
            cy.getById(this.navHeaderId).contains(section).should("exist");
        });
        return this;
    }

    unableToView(sections: string[]) {
        sections.forEach((section) => {
            cy.getById(this.navHeaderId).contains(section).should("not.exist");
        });
        return this;
    }

    unableToViewTheNavBar() {
        cy.getById(this.navHeaderId).should("not.exist");
        return this;
    }
}

const navBar = new NavBar();

export default navBar;
