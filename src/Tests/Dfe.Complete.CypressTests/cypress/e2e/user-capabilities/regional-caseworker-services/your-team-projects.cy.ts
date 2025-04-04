import { before } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import yourTeamProjectsRCSViewTable from "cypress/pages/projects/tables/yourTeamProjectsRCSViewTable";
import {
    regionalCaseworkerUserAdId,
    regionalCaseworkerUserEmail,
    regionalCaseWorkerUserName,
} from "cypress/constants/stringTestConstants";
import { Username } from "cypress/constants/cypressConstants";

const project = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111396);
const schoolName = "Blacon High School, A Specialist Sports College";
const teammatesProject = ProjectBuilder.createConversionProjectRequest(
    new Date("2026-04-01"),
    111400,
    regionalCaseworkerUserAdId,
);
const teammatesSchoolName = "The Heath School";

describe("Regional caseworker services user - View your team projects", () => {
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
            .schoolHasLocalAuthority(schoolName, "Cheshire West and Chester\n")
            .schoolHasRegion(schoolName, "North West")
            .schoolHasAssignedTo(schoolName, Username)
            .schoolHasProjectType(schoolName, "Conversion")
            .schoolHasFormAMatProject(teammatesSchoolName, "No")
            .schoolHasConversionOrTransferDate(teammatesSchoolName, "Apr 2026")
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
            .schoolHasAssignedTo(teammatesSchoolName, regionalCaseWorkerUserName)
            .schoolHasProjectType(teammatesSchoolName, "Conversion")
            .schoolHasFormAMatProject(teammatesSchoolName, "No")
            .schoolHasConversionOrTransferDate(teammatesSchoolName, "Apr 2026")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it.skip("Should be able to view my team projects that are new", () => {
        // not implemented
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

    it.skip("Should be able to view my team projects by user and all a user's projects", () => {
        // not implemented
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
            .hasTableHeader("Region") // this has been changed from Team to Region for .NET
            .hasTableHeader("Type of project")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project completion date")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it("Should NOT be able to view handed my team projects that are handed over", () => {
        yourTeamProjects.doesNotContainFilter("Handed over");
        // not implemented:
        // cy.visit("/projects/team/handed-over").notAuthorisedToPerformAction();
    });
});
