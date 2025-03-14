import projectRemover from "../../api/projectRemover";
import projectApi from "../../api/projectApi";
import {ProjectBuilder} from "../../api/projectBuilder";
import yourProjects from "../../pages/projects/yourProjects";
import { projectTable } from "../../pages/projects/tables/projectTable";
import yourProjectsInProgressTable from "../../pages/projects/tables/yourProjectsInProgressTable";

const project = ProjectBuilder.createConversionProjectRequest(111394);
const schoolName = "Farnworth Church of England Controlled Primary School";

describe("View your projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createProject(project);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.acceptCookies();
        cy.visit(`/projects/yours/in-progress`);
    });

    it("Should be able to view newly created conversion project in Your projects", () => {
        yourProjects
            .containsHeading("Your projects in progress")
            .goToNextPageUntilFieldIsVisible(schoolName);
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
            .schoolHasIncomingTrust(schoolName, "AURORA ACADEMIES TRUST")
            .schoolHasOutgoingTrust(schoolName, "None")
            .schoolHasLocalAuthority(schoolName, "Halton")
            .schoolHasConversionOrTransferDate(schoolName, "Nov 2026");
        yourProjectsInProgressTable.goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    // awaiting BUG 201976 to be able to create the different project types
    it.skip("Should be able to view newly created transfer project in Your projects", () => {});
    it.skip("Should be able to view newly created conversion form a MAT project in Your projects", () => {});
    it.skip("Should be able to view newly created transfer form a MAT project in Your projects", () => {});
});