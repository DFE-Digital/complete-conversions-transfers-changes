import BasePage from "cypress/pages/basePage";

class NavBar extends BasePage {
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
        this.expectToView(sections, true);
        return this;
    }

    unableToView(sections: string[]) {
        this.expectToView(sections, false);
        return this;
    }

    expectToView(sections: string[], visible: boolean) {
        sections.forEach((section) => {
            cy.getById(this.navHeaderId)
                .contains(section)
                .should(visible ? "exist" : "not.exist");
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
