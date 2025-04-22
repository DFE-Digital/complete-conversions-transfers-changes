import {
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewProjectExports,
    shouldNotHaveAccessToViewYourTeamUnassignedProjects,
} from "cypress/support/reusableTests";
import { beforeEach } from "mocha";
import { cypressUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";

describe("Capabilities and permissions of the regional delivery officer user", () => {
    beforeEach(() => {
        cy.login(cypressUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user your projects in progress after login", () => {
        cy.url().should("include", "/projects/yours/in-progress");
    });

    it("Should be able to view 'Your projects', 'Your team projects', 'All projects' and 'Groups' sections", () => {
        navBar.ableToView(["Your projects", "Your team projects", "All projects", "Groups"]);
    });

    it("Should NOT be able to view 'Service support' section", () => {
        navBar.unableToView(["Service support"]);
    });

    it("Should NOT have access to view Your team projects -> unassigned projects", () => {
        shouldNotHaveAccessToViewYourTeamUnassignedProjects();
    });

    it("Should NOT have access to view All projects -> exports", () => {
        shouldNotHaveAccessToViewProjectExports();
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });
});
