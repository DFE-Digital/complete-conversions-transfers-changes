import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import yourProjects from "cypress/pages/projects/yourProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import yourProjectsInProgressTable from "cypress/pages/projects/tables/yourProjectsInProgressTable";

const project = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111394);
const schoolName = "Farnworth Church of England Controlled Primary School";

describe("View your projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createConversionProject(project);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.acceptCookies();
        cy.visit(`/projects/yours/in-progress`);
    });

    it("Should be able to view newly created conversion project in Your projects", () => {
        yourProjects.containsHeading("Your projects in progress").goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Type of project")
            .hasTableHeader("Form a MAT project")
            .hasTableHeader("Incoming trust")
            .hasTableHeader("Outgoing trust")
            .hasTableHeader("Local authority")
            .hasTableHeader("Conversion or transfer date");
        yourProjectsInProgressTable
            .schoolHasUrn(schoolName, `${project.urn.value}`)
            .schoolHasTypeOfProject(schoolName, "Conversion")
            .schoolHasFormAMatProject(schoolName, "No")
            // .schoolHasIncomingTrust(schoolName, trust) // bug 208086
            .schoolHasOutgoingTrust(schoolName, "None")
            .schoolHasLocalAuthority(schoolName, "Halton")
            .schoolHasConversionOrTransferDate(schoolName, "Apr 2026");
        yourProjectsInProgressTable.goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    // awaiting BUG 201976 to be able to create the different project types
    it.skip("Should be able to view newly created transfer project in Your projects", () => {});
    it.skip("Should be able to view newly created conversion form a MAT project in Your projects", () => {});
    it.skip("Should be able to view newly created transfer form a MAT project in Your projects", () => {});
});
