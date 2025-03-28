import { before } from "mocha";
import projectRemover from "../../api/projectRemover";
import projectApi from "../../api/projectApi";
import { ProjectBuilder } from "../../api/projectBuilder";
import navBar from "../../pages/navBar";
import yourTeamProjects from "../../pages/projects/yourTeamProjects";
import yourTeamProjectsRCSViewTable from "../../pages/projects/tables/yourTeamProjectsRCSViewTable";

const project = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111396);
const schoolName = "Blacon High School, A Specialist Sports College";
//todo create regional caseworker service account
const regionalCaseworkerUserEmail = "todo";
const regionalCaseworkerUserAdId = "todo";
const regionalCaseWorkerUserName = "todo";
const teammatesProject = ProjectBuilder.createConversionProjectRequest(
    new Date("2026-04-01"),
    111397,
    regionalCaseworkerUserAdId,
);
const teammatesSchoolName = "Queen's Park High School";

describe.skip("Regional caseworker services user - View your team projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${teammatesProject.urn.value}`);
        projectApi.createProject(project);
        projectApi.createProject(teammatesProject, regionalCaseworkerUserEmail);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalCaseworkServices" });
        cy.acceptCookies();
        cy.visit("/projects/team/in-progress");
    });

    it("Should be able to view my project in my team project listings in progress", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects.containsHeading("Your team projects in progress").goToNextPageUntilFieldIsVisible(schoolName);
        yourTeamProjectsRCSViewTable
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

    it("Should be able to view my teammate's project in my team project listings in progress", () => {
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToNextPageUntilFieldIsVisible(teammatesSchoolName);
        yourTeamProjectsRCSViewTable
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
        yourTeamProjectsRCSViewTable
            .schoolIsFirstInTable(teammatesSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Created at date")
            .hasTableHeader("Region")
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
            .goToNextPageUntilFieldIsVisible(regionalCaseWorkerUserName);
        yourTeamProjectsRCSViewTable
            .hasTableHeader("User name")
            .hasTableHeader("Email")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .goTo(regionalCaseWorkerUserName);
        yourTeamProjects.containsHeading(`Projects assigned to ${regionalCaseWorkerUserName}`);
        yourTeamProjectsRCSViewTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .contains(teammatesSchoolName)
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it.skip("Should be able to view my team projects that are completed", () => {
        // not implemented
        yourTeamProjects.filterProjects("Completed").containsHeading("Your team completed projects");
        yourTeamProjectsRCSViewTable
            .schoolIsFirstInTable(teammatesSchoolName)
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Team") // check?
            .hasTableHeader("Type of project")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project completion date")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });
});
