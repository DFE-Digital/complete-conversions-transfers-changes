import { before } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import yourTeamProjectsRDOViewTable from "cypress/pages/projects/tables/yourTeamProjectsRCSViewTable";
import { rdoLondonUserAdId, rdoLondonUserEmail, rdoLondonUserName } from "cypress/constants/stringTestConstants";

const project = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111394);
const schoolName = "Farnworth Church of England Controlled Primary School";
const teammatesLondonRegionProject = ProjectBuilder.createConversionProjectRequest(
    new Date("2026-04-01"),
    100830,
    rdoLondonUserAdId,
);
const teammatesLondonSchoolName = "St John's and St Clement's Church of England Primary School";
describe("Regional delivery officer (London) user - View your team projects (projects with London region)", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${teammatesLondonRegionProject.urn.value}`);
        projectApi.createProject(project);
        projectApi.createProject(teammatesLondonRegionProject, rdoLondonUserEmail);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.acceptCookies();
        cy.visit("/projects/team/in-progress");
    });

    it("Should be able to view my project in my team project listings in progress", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects.containsHeading("Your team projects in progress").goToNextPageUntilFieldIsVisible(schoolName);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Region")
            .hasTableHeader("Assigned to")
            .hasTableHeader("Project type")
            .hasTableHeader("Form a MAT project")
            .hasTableHeader("Conversion or transfer date")
            .schoolHasUrn(schoolName, `${project.urn.value}`)
            .schoolHasLocalAuthority(schoolName, "Halton")
            .schoolHasRegion(schoolName, "North West")
            .schoolHasAssignedTo(schoolName, "Regional Delivery Officer")
            .schoolHasProjectType(schoolName, "Conversion")
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view my teammate's project (who is in the same region) in my team project listings in progress", () => {
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToNextPageUntilFieldIsVisible(teammatesLondonSchoolName);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Region")
            .hasTableHeader("Assigned to")
            .hasTableHeader("Project type")
            .hasTableHeader("Form a MAT project")
            .hasTableHeader("Conversion or transfer date")
            .schoolHasUrn(teammatesLondonSchoolName, `${project.urn.value}`)
            .schoolHasLocalAuthority(teammatesLondonSchoolName, "Halton")
            .schoolHasRegion(teammatesLondonSchoolName, "North West")
            .schoolHasAssignedTo(teammatesLondonSchoolName, "Regional Delivery Officer")
            .schoolHasProjectType(teammatesLondonSchoolName, "Conversion")
            .goTo(teammatesLondonSchoolName);
        // projectDetailsPage.containsHeading(teammatesLondonSchoolName); // not implemented
    });

    it("Should be able to view my team projects that are new", () => {
        yourTeamProjects.filterProjects("New").containsHeading("Your team new projects");
        yourTeamProjectsRDOViewTable
            .schoolIsFirstInTable(teammatesLondonSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Created at date")
            .hasTableHeader("Team")
            .hasTableHeader("Assigned to")
            .hasTableHeader("Project type")
            .hasTableHeader("Conversion or transfer date")
            .goTo(teammatesLondonSchoolName);
        // projectDetailsPage.containsHeading(teammatesLondonSchoolName); // not implemented
    });

    it("Should be able to view my team projects by user and all a user's projects", () => {
        yourTeamProjects
            .filterProjects("New")
            .containsHeading("Your team projects by user")
            .goToNextPageUntilFieldIsVisible(rdoLondonUserName);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("User name")
            .hasTableHeader("Email")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .goTo(rdoLondonUserName);
        yourTeamProjects.containsHeading(`Projects assigned to ${rdoLondonUserName}`);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .contains(teammatesLondonSchoolName)
            .goTo(teammatesLondonSchoolName);
        // projectDetailsPage.containsHeading(teammatesLondonSchoolName); // not implemented
    });

    it.skip("Should be able to view my team projects that are handed over", () => {
        // not implemented
        yourTeamProjects.filterProjects("Handed over").containsHeading("Handed over");
        yourTeamProjectsRDOViewTable
            .schoolIsFirstInTable(teammatesLondonSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .hasTableHeader("Assigned to")
            .goTo(teammatesLondonSchoolName);
        // projectDetailsPage.containsHeading(teammatesLondonSchoolName); // not implemented
    });

    it.skip("Should be able to view my team projects that are completed", () => {
        // not implemented, unable to move project to completed
        yourTeamProjects.filterProjects("Completed").containsHeading("Your team completed projects");
        yourTeamProjectsRDOViewTable
            .schoolIsFirstInTable(teammatesLondonSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Team")
            .hasTableHeader("Type of project")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project completion date")
            .goTo(teammatesLondonSchoolName);
        // projectDetailsPage.containsHeading(teammatesLondonSchoolName); // not implemented
    });

    it("Should NOT be able to view unassigned projects", () => {
        yourTeamProjects.doesNotContainFilter("Unassigned");
        cy.visit("/projects/team/unassigned").notAuthorisedToPerformAction();
    });
});
