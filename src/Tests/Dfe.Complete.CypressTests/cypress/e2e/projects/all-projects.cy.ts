import navBar from "cypress/pages/navBar";
import allProjects from "cypress/pages/projects/allProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { before, beforeEach } from "mocha";
import { nextMonth, trust } from "cypress/constants/stringTestConstants";
import { Username } from "cypress/constants/cypressConstants";
import { shouldOnlyBeAbleToViewNextMonthOfProjects } from "../../support/reusableTests";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import allProjectsInProgressTable from "cypress/pages/projects/tables/allProjectsInProgressTable";

const project = ProjectBuilder.createConversionProjectRequest(nextMonth);
let projectId: string;
const schoolName = "St Chad's Catholic Primary School";
const region = "West Midlands";
const localAuthority = "Dudley Metropolitan Borough Council";
const transferProject = ProjectBuilder.createTransferProjectRequest();
const transferSchoolName = "Abbey College Manchester";

describe("View all projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        // projectRemover.removeProjectIfItExists(`${transferProject.urn.value}`);
        projectApi.createProject(project).then((response) => (projectId = response.value));
        // projectApi.createProject(transferProject);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.acceptCookies();
        cy.visit(`/projects/all/in-progress/all`);
    });

    it.skip("Should be able to view my team projects that are handed over", () => {
        // not implemented
        navBar.goToAllProjects();
        allProjects.filterProjects("Handover").containsHeading("Projects to handover");
    });

    it("Should be able to view newly created conversion project in All projects in progress and Conversions projects", () => {
        const nextMonthString = `${nextMonth.toLocaleString("default", { month: "short" })} ${nextMonth.getFullYear()}`;
        navBar.goToAllProjects();
        allProjects.containsHeading("All projects in progress").goToNextPageUntilFieldIsVisible(schoolName);
        allProjectsInProgressTable
            .contains(schoolName)
            .schoolHasUrn(schoolName, `${project.urn.value}`)
            .schoolHasConversionOrTransferDate(schoolName, nextMonthString)
            .schoolHasProjectType(schoolName, "Conversion")
            .schoolHasFormAMatProject(schoolName, "No")
            .schoolHasAssignedTo(schoolName, Username);
        allProjects
            .viewConversionsProjects()
            .containsHeading("All conversions in progress")
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable.goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it.skip("Should be able to view newly created transfer project in All projects in progress and Transfers projects", () => {
        // 205986 unable to create transfer project
        navBar.goToAllProjects();
        allProjects.goToNextPageUntilFieldIsVisible(transferSchoolName);
        allProjectsInProgressTable
            .contains(transferSchoolName)
            .schoolHasUrn(transferSchoolName, `${transferProject.urn.value}`)
            .schoolHasConversionOrTransferDate(transferSchoolName, "Mar 2026")
            .schoolHasProjectType(transferSchoolName, "Transfer")
            .schoolHasFormAMatProject(transferSchoolName, "No")
            .schoolHasAssignedTo(transferSchoolName, Username);
        allProjects
            .viewTransfersProjects()
            .containsHeading("All transfers in progress")
            .goToNextPageUntilFieldIsVisible(transferSchoolName);
        projectTable.goTo(transferSchoolName);
        // projectDetailsPage.containsHeading(transferSchoolName); // not implemented
    });

    it.skip("Should be able to view all Conversions projects by month - next month only", () => {
        shouldOnlyBeAbleToViewNextMonthOfProjects(schoolName, project);
    });

    it("Should be able to view all projects by region and all a region's projects", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By region").containsHeading("All projects by region");
        projectTable
            .hasTableHeader("Region")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .contains(region)
            .filterBy(region);
        allProjects
            // .containsHeading(`Projects for ${region}`); //bug 205144
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .hasTableHeader("Assigned to")
            .contains(schoolName)
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view all projects by user and all a user's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By user")
            .containsHeading("All projects by user")
            .goToNextPageUntilFieldIsVisible(Username);
        projectTable
            .hasTableHeader("User name")
            .hasTableHeader("Email")
            .hasTableHeader("Team")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .goToUserProjects(Username);
        allProjects.containsHeading(`Projects for ${Username}`);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .contains(schoolName)
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName) // not implemented
    });

    it("Should be able to view all projects by trust and all a trust's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By trust")
            .containsHeading("All projects by trust")
            .goToNextPageUntilFieldIsVisible(trust);
        projectTable
            .hasTableHeader("Trust")
            .hasTableHeader("Group identifier")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .filterBy(trust);
        allProjects
            // .containsHeading(`Projects for ${trust}`) // bug 208086
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .hasTableHeader("Assigned to")
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view all projects by local authority and all a local authority's projects", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By local authority").containsHeading("All projects by local authority");
        projectTable
            .hasTableHeader("Local authority")
            .hasTableHeader("Code")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers");
        allProjects.goToNextPageUntilFieldIsVisible(localAuthority);
        projectTable.filterBy(localAuthority);
        allProjects.containsHeading(`Projects for ${localAuthority}`);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .hasTableHeader("Assigned to")
            .contains(schoolName)
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it.skip("Should be able to view all completed projects", () => {
        // project completion not implemented
        cy.visit(`projects/${projectId}/tasks`);
        // click complete project on tasks page
        navBar.goToAllProjects();
        allProjects
            .filterProjects("Completed")
            .containsHeading("All completed projects")
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Local authority")
            .hasTableHeader("Team")
            .hasTableHeader("Type of project")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project completion date")
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });
});
