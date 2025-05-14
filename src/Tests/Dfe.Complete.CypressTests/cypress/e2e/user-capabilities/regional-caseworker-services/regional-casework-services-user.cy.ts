import {
    shouldNotHaveAccessToViewAndEditUsers,
    shouldNotHaveAccessToViewHandedOverProjects,
    shouldNotHaveAccessToViewProjectExports,
    shouldNotHaveAccessToViewYourTeamUnassignedProjects,
} from "cypress/support/reusableTests";
import { beforeEach } from "mocha";
import { regionalCaseworkerUser } from "cypress/constants/cypressConstants";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import allProjects from "cypress/pages/projects/allProjects";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";
import { nextMonthLong } from "cypress/constants/stringTestConstants";
import yourProjects from "cypress/pages/projects/yourProjects";

describe("Capabilities and permissions of the regional casework services user", () => {
    beforeEach(() => {
        cy.login(regionalCaseworkerUser);
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user your projects in progress after login", () => {
        cy.url().should("include", "/projects/yours/in-progress");
    });

    it("Should be able to view 'Your projects', 'Your team projects', 'All projects' and 'Groups' sections and filters", () => {
        navBar.ableToView(["Your projects", "Your team projects", "All projects", "Groups"]);
        navBar.goToYourProjects();
        yourProjects.ableToViewFilters(["In progress", "Added by you", "Completed"]);
        navBar.goToYourTeamProjects();
        yourTeamProjects.ableToViewFilters(["In progress", "New", "By user", "Completed"]);
        navBar.goToAllProjects();
        allProjects.ableToViewFilters([
            "In progress",
            "By month",
            "By region",
            "By user",
            "By trust",
            "By local authority",
            "Completed",
            "Statistics"
        ]);
    });

    it("Should NOT be able to view 'Service support' section", () => {
        navBar.unableToView(["Service support"]);
    });

    it("Should be able to view all projects by month - next month only", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By month").containsHeading(nextMonthLong);
        projectsByMonthPage.filterDoesNotExist();
    });

    it("Should NOT have access to view Your team projects -> unassigned projects", () => {
        shouldNotHaveAccessToViewYourTeamUnassignedProjects();
    });

    it("Should NOT be able to view Your team projects -> handed over projects", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects.unableToViewFilter("Handed over");
    });

    it("Should NOT have access to view All projects -> handed over projects", () => {
        shouldNotHaveAccessToViewHandedOverProjects();
    });

    it("Should NOT have access to view All projects -> exports", () => {
        shouldNotHaveAccessToViewProjectExports();
    });

    it.skip("Should NOT have access to view and edit users", () => {
        // not implemented
        shouldNotHaveAccessToViewAndEditUsers();
    });
});
