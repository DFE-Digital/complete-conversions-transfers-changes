import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotBeAbleToViewAndEditLocalAuthorities,
    shouldNotBeAbleToViewAndEditUsers,
    shouldNotBeAbleToViewConversionURNs,
    shouldNotBeAbleToViewYourProjects,
} from "../../support/reusableTests";

describe.skip("Capabilities and permissions of the business support user", () => {
    beforeEach(() => {
        cy.login({ role: "BusinessSupport" });
        cy.acceptCookies();
        cy.visit("/");
    });

    it("Should be able to view multiple months of projects within a specified date range", () => {
        shouldBeAbleToViewMultipleMonthsOfProjects();
    });

    it.skip("Should be able to view and download csv reports from the export section", () => {
        // not implemented 187516
        shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection();
    });

    it("Should NOT be able view your projects", () => {
        shouldNotBeAbleToViewYourProjects();
    });

    it("Should NOT be able to create a project", () => {
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
