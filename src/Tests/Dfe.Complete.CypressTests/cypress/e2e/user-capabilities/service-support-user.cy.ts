import { beforeEach } from "mocha";
import {
    shouldBeAbleToViewAndDownloadCsvReportsFromTheExportSection,
    shouldBeAbleToViewMultipleMonthsOfProjects,
    shouldNotBeAbleToBeAssignedAProject,
    shouldNotBeAbleToCreateAProject,
    shouldNotBeAbleToViewYourProjects,
} from "../../support/reusableTests";

describe.skip("Capabilities and permissions of the service support user", () => {
    beforeEach(() => {
        cy.login({ role: "ServiceSupport" });
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

    it("Should NOT be able view your projects", () => {
        shouldNotBeAbleToViewYourProjects();
    });

    it("Should NOT be able to create a project", () => {
        shouldNotBeAbleToCreateAProject();
    });

    it.skip("Should NOT be able to be assigned a project", () => {
        shouldNotBeAbleToBeAssignedAProject();
    });
});
