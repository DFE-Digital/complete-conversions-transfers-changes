import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotHaveAccessToViewYourTeamUnassignedProjects,
} from "cypress/support/reusableTests";
import navBar from "cypress/pages/navBar";
import { serviceSupportUser } from "cypress/constants/cypressConstants";

// skipped as visiting / goes to service support that is not implemented yet
describe.skip("Capabilities and permissions of the service support user", () => {
    beforeEach(() => {
        cy.login({ activeDirectoryId: serviceSupportUser.adId });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should direct user to all Service support -> without academy URNs after login", () => {
        cy.url().should("include", "/projects/service-support/without-academy-urn");
    });

    it("Should be able to view 'All projects', 'Groups' and 'Service support' sections", () => {
        navBar.ableToView(["All projects", "Groups", "Service support"]);
    });

    it("Should NOT be able to view 'Your projects', 'Your team projects' sections", () => {
        navBar.unableToView(["Your projects", "Your team projects"]);
    });

    it("Should NOT have access to view Your team projects -> unassigned projects", () => {
        shouldNotHaveAccessToViewYourTeamUnassignedProjects();
    });

    it.skip("Should be able to view multiple months of projects within a specified date range", () => {
        // not implemented 187514
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it.skip("Should be able to view and download csv reports from the export section", () => {
        // not implemented 187516
        shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection();
    });

    it.skip("Should be able to review projects newly handed over from prepare", () => {
        // not implemented 187511
    });

    it.skip("Should be able to soft delete a project", () => {
        // not implemented
    });

    it.skip("Should be able to create users", () => {
        // not implemented 187527
    });

    it.skip("Should be able to edit users", () => {
        // not implemented 187527
    });

    it.skip("Should be able to create local authority details", () => {
        // not implemented 187525
    });

    it.skip("Should be able to edit local authority details", () => {
        // not implemented 187525
    });

    it.skip("Should NOT be able to create a project", () => {
        // not implemented
        shouldNotBeAbleToCreateAProject();
    });

    it.skip("Should NOT be able to be assigned a project", () => {
        // not implemented
        shouldNotBeAbleToBeAssignedAProject();
    });
});
