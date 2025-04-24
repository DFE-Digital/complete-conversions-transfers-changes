import { before } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { cypressUser, rdoLondonUser } from "cypress/constants/cypressConstants";
import yourTeamProjectsRDOViewTable from "cypress/pages/projects/tables/yourTeamProjectsRDOViewTable";

const team = "London";
const myLondonProject = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 143659);
const myLondonSchoolName = "City of London Academy, Highgate Hill";
const teammatesLondonRegionProject = ProjectBuilder.createConversionProjectRequest(
    new Date("2026-04-01"),
    100830,
    rdoLondonUser.adId,
);
const teammatesLondonSchoolName = "St John's and St Clement's Church of England Primary School";
describe("Regional delivery officer (London) user - View your team projects (projects with London region)", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${myLondonProject.urn.value}`);
        projectRemover.removeProjectIfItExists(`${teammatesLondonRegionProject.urn.value}`);
        projectApi.createConversionProject(myLondonProject);
        projectApi.createConversionProject(teammatesLondonRegionProject, rdoLondonUser.email);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit("/projects/team/in-progress");
    });

    it("Should be able to view my (London) project in my team project listings in progress", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToNextPageUntilFieldIsVisible(myLondonSchoolName);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Team")
            .hasTableHeader("Assigned to")
            .hasTableHeader("Project type")
            .hasTableHeader("Form a MAT project")
            .hasTableHeader("Conversion or transfer date")
            .schoolHasUrn(myLondonSchoolName, `${myLondonProject.urn.value}`)
            .schoolHasLocalAuthority(myLondonSchoolName, "Islington")
            .schoolHasTeam(myLondonSchoolName, team)
            .schoolHasAssignedTo(myLondonSchoolName, cypressUser.username)
            .schoolHasProjectType(myLondonSchoolName, "Conversion")
            .goTo(myLondonSchoolName);
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
            .hasTableHeader("Team")
            .hasTableHeader("Assigned to")
            .hasTableHeader("Project type")
            .hasTableHeader("Form a MAT project")
            .hasTableHeader("Conversion or transfer date")
            .schoolHasUrn(teammatesLondonSchoolName, `${teammatesLondonRegionProject.urn.value}`)
            .schoolHasLocalAuthority(teammatesLondonSchoolName, "Southwark")
            .schoolHasTeam(teammatesLondonSchoolName, team)
            .schoolHasAssignedTo(teammatesLondonSchoolName, rdoLondonUser.username)
            .schoolHasProjectType(teammatesLondonSchoolName, "Conversion")
            .goTo(teammatesLondonSchoolName);
        // projectDetailsPage.containsHeading(teammatesLondonSchoolName); // not implemented
    });

    it.skip("Should be able to view my team projects that are new", () => {
        // not implemented
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

    it.skip("Should be able to view my team projects by user and all a user's projects", () => {
        // not implemented
        yourTeamProjects
            .filterProjects("New")
            .containsHeading("Your team projects by user")
            .goToNextPageUntilFieldIsVisible(rdoLondonUser.username);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("User name")
            .hasTableHeader("Email")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .goTo(rdoLondonUser.username);
        yourTeamProjects.containsHeading(`Projects assigned to ${rdoLondonUser.username}`);
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
        // not implemented:
        // cy.visit("/projects/team/unassigned").notAuthorisedToPerformAction();
    });
});
