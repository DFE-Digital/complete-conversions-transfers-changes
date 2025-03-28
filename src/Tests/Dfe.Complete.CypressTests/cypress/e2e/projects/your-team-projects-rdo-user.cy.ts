import { before } from "mocha";
import projectRemover from "../../api/projectRemover";
import projectApi from "../../api/projectApi";
import navBar from "../../pages/navBar";
import yourTeamProjects from "../../pages/projects/yourTeamProjects";
import { ProjectBuilder } from "../../api/projectBuilder";
import yourTeamProjectsRDOViewTable from "../../pages/projects/tables/yourTeamProjectsRCSViewTable";

const project = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111394);
const schoolName = "Farnworth Church of England Controlled Primary School";
// todo create regional delivery officer service account in London team
const londonUserEmail = "todo";
const londonUserAdId = "todo";
const londonUserName = "todo";
const teammatesProject = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111395, londonUserAdId);
const teammatesSchoolName = "Kingsway High School";
describe.skip("Regional delivery officer user - View your team projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${teammatesProject.urn.value}`);
        projectApi.createProject(project);
        projectApi.createProject(teammatesProject, londonUserEmail);
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
            .goToNextPageUntilFieldIsVisible(teammatesSchoolName);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Region")
            .hasTableHeader("Assigned to")
            .hasTableHeader("Project type")
            .hasTableHeader("Form a MAT project")
            .hasTableHeader("Conversion or transfer date")
            .schoolHasUrn(teammatesSchoolName, `${project.urn.value}`)
            .schoolHasLocalAuthority(teammatesSchoolName, "Halton")
            .schoolHasRegion(teammatesSchoolName, "North West")
            .schoolHasAssignedTo(teammatesSchoolName, "Regional Delivery Officer")
            .schoolHasProjectType(teammatesSchoolName, "Conversion")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it("Should be able to view my team projects that are new", () => {
        yourTeamProjects.filterProjects("New").containsHeading("Your team new projects");
        yourTeamProjectsRDOViewTable
            .schoolIsFirstInTable(teammatesSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Created at date")
            .hasTableHeader("Team")
            .hasTableHeader("Assigned to")
            .hasTableHeader("Project type")
            .hasTableHeader("Conversion or transfer date")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it("Should be able to view my team projects by user and all a user's projects", () => {
        yourTeamProjects
            .filterProjects("New")
            .containsHeading("Your team projects by user")
            .goToNextPageUntilFieldIsVisible(londonUserName);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("User name")
            .hasTableHeader("Email")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .goTo(londonUserName);
        yourTeamProjects.containsHeading(`Projects assigned to ${londonUserName}`);
        yourTeamProjectsRDOViewTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .contains(teammatesSchoolName)
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it.skip("Should be able to view my team projects that are handed over", () => {
        // not implemented
        yourTeamProjects.filterProjects("Handed over").containsHeading("Handed over");
        yourTeamProjectsRDOViewTable
            .schoolIsFirstInTable(teammatesSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .hasTableHeader("Assigned to")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it("Should be able to view my team projects that are completed", () => {
        // not implemented
        yourTeamProjects.filterProjects("Completed").containsHeading("Your team completed projects");
        yourTeamProjectsRDOViewTable
            .schoolIsFirstInTable(teammatesSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Team")
            .hasTableHeader("Type of project")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project completion date")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });
});
