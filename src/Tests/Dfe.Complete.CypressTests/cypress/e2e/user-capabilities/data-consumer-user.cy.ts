import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotBeAbleToViewAndEditLocalAuthorities,
    shouldNotBeAbleToViewAndEditUsers,
    shouldNotBeAbleToViewConversionURNs,
    shouldNotBeAbleToViewHandedOverProjects,
    shouldNotBeAbleToViewYourProjects,
    shouldNotBeAbleToViewYourTeamProjects,
} from "cypress/support/reusableTests";
import { dataConsumerUser } from "cypress/constants/cypressConstants";

describe("Capabilities and permissions of the data consumer user", () => {
    beforeEach(() => {
        cy.login({ activeDirectoryId: dataConsumerUser.adId });
        cy.acceptCookies();
        cy.visit("/");
    });

    it.skip("Should be able to view multiple months of projects within a specified date range", () => {
        // not implemented 187514
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it.skip("Should be able to view and download csv reports from the export section", () => {
        // not implemented 187516
        shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection();
    });

    it.skip("Should NOT be able to view your projects", () => {
        // not implemented 208988
        shouldNotBeAbleToViewYourProjects();
    });

    it("Should NOT be able to view your team projects", () => {
        shouldNotBeAbleToViewYourTeamProjects();
    });

    it.skip("Should NOT be able to view handed over projects", () => {
        // not implemented
        shouldNotBeAbleToViewHandedOverProjects();
    });

    it.skip("Should NOT be able to create a project", () => {
        // not implemented
        shouldNotBeAbleToCreateAProject();
    });

    it.skip("Should NOT be able to be assigned a project", () => {
        // not implemented
        shouldNotBeAbleToBeAssignedAProject();
    });

    it.skip("Should NOT be able to soft delete projects", () => {
        // not implemented
    });

    it.skip("Should NOT be able to view and edit users", () => {
        // not implemented
        shouldNotBeAbleToViewAndEditUsers();
    });

    it.skip("Should NOT be able to view and edit local authorities", () => {
        // not implemented
        shouldNotBeAbleToViewAndEditLocalAuthorities();
    });

    it.skip("Should NOT be able to view conversion URNs", () => {
        // not implemented
        shouldNotBeAbleToViewConversionURNs();
    });
});
