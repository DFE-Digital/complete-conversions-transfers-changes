import navBar from "cypress/pages/navBar";
import allProjects from "cypress/pages/projects/allProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { before, beforeEach } from "mocha";
import { nextMonth, nextMonthLong, trust } from "cypress/constants/stringTestConstants";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectRemover from "cypress/api/projectRemover";
import { cypressUser } from "cypress/constants/cypressConstants";
import projectsByMonthPage from "cypress/pages/projects/projectsByMonthPage";

const project = ProjectBuilder.createConversionProjectRequest(nextMonth);
let projectId: string;
const schoolName = "St Chad's Catholic Primary School";
const region = "West Midlands";
const localAuthority = "Dudley Metropolitan Borough Council";
const transferProject = ProjectBuilder.createTransferProjectRequest(nextMonth);
const transferSchoolName = "Abbey College Manchester";

describe("View all projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${transferProject.urn.value}`);
        projectApi.createConversionProject(project).then((response) => (projectId = response.value));
        projectApi.createTransferProject(transferProject);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`/projects/all/in-progress/all`);
    });

    it.skip("Should be able to view my team projects that are handed over", () => {
        // not implemented
        navBar.goToAllProjects();
        allProjects.filterProjects("Handover").containsHeading("Projects to handover");
    });

    it("Should be able to view newly created conversion project in All projects in progress and Conversions projects", () => {
        navBar.goToAllProjects();
        allProjects.containsHeading("All projects in progress").goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Conversion or transfer date",
                "Project type",
                "Form a MAT project?",
                "Assigned to",
            ])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            .columnHasValue("Conversion or transfer date", nextMonthLong)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username);
        allProjects
            .viewConversionsProjects()
            .containsHeading("All conversions in progress")
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion date", "Form a MAT project?", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            .columnHasValue("Conversion date", nextMonthLong)
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view newly created transfer project in All projects in progress and Transfers projects", () => {
        navBar.goToAllProjects();
        allProjects.containsHeading("All projects in progress").goToNextPageUntilFieldIsVisible(transferSchoolName);
        projectTable
            .withSchool(transferSchoolName)
            .columnHasValue("URN", `${transferProject.urn.value}`)
            .columnHasValue("Conversion or transfer date", nextMonthLong)
            .columnHasValue("Project type", "Transfer")
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username);
        allProjects
            .viewTransfersProjects()
            .containsHeading("All transfers in progress")
            .goToNextPageUntilFieldIsVisible(transferSchoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Transfer date", "Form a MAT project?", "Assigned to"])
            .withSchool(transferSchoolName)
            .columnHasValue("URN", `${transferProject.urn.value}`)
            .columnHasValue("Transfer date", nextMonthLong)
            .columnHasValue("Form a MAT project?", "No")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(transferSchoolName);
        // projectDetailsPage.containsHeading(transferSchoolName); // not implemented
    });

    it("Should be able to view all Conversions projects by month - next month only", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By month").containsHeading(nextMonthLong);
        projectsByMonthPage.filterDoesNotExist();
        projectTable
            .hasTableHeaders([
                "School and URN",
                "Region",
                "Local authority",
                "Incoming trust",
                "All conditions met",
                "Confirmed date (Original date)",
            ])
            .contains(`${schoolName} ${project.urn.value}`)
            .withSchool(`${schoolName} ${project.urn.value}`)
            .columnHasValue("Region", region)
            .columnHasValue("Local authority", localAuthority)
            // .columnHasValue("Incoming trust", trust) // bug 208086
            .columnHasValue("All conditions met", "Not yet")
            .columnHasValue("Confirmed date (Original date)", nextMonthLong)
            .goTo(`${schoolName} ${project.urn.value}`);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view all Transfer projects by month - next month only", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By month").containsHeading(nextMonthLong);
        projectsByMonthPage.filterDoesNotExist().viewTransferProjects();
        projectTable
            .hasTableHeaders([
                "School and URN",
                "Region",
                "Outgoing trust",
                "Incoming trust",
                "Authority to proceed",
                "Confirmed date (Original date)",
            ])
            .contains(`${transferSchoolName} ${transferProject.urn.value}`)
            .withSchool(`${transferSchoolName} ${transferProject.urn.value}`)
            .columnHasValue("Region", region)
            .columnHasValue("Outgoing trust", trust)
            .columnHasValue("Incoming trust", "5 Dimensions Trust")
            .columnHasValue("Authority to proceed", "Not yet")
            .columnHasValue("Confirmed date (Original date)", `${nextMonthLong} (${nextMonthLong})`)
            .goTo(`${transferSchoolName} ${transferProject.urn.value}`);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view all projects by region and all a region's projects", () => {
        navBar.goToAllProjects();
        allProjects.filterProjects("By region").containsHeading("All projects by region");
        projectTable.hasTableHeaders(["Region", "Conversions", "Transfers"]).contains(region).filterBy(region);
        allProjects.containsHeading(`Projects for ${region}`).goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            .columnHasValue("Conversion or transfer date", nextMonthLong)
            .columnHasValue("Project type", "Conversion")
            .columnContainsValue("Assigned to", cypressUser.username)
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view all projects by user and all a user's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By user")
            .containsHeading("All projects by user")
            .goToNextPageUntilFieldIsVisible(cypressUser.username);
        projectTable
            .hasTableHeaders(["User name", "Email", "Team", "Conversions", "Transfers"])
            // .withUser(cypressUser.username)
            // .columnHasValue("Email", cypressUser.email)
            // .columnHasValue("Team", "London")
            .goToUserProjects(cypressUser.username);
        allProjects.containsHeading(`Projects for ${cypressUser.username}`);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type"])
            .contains(schoolName)
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            // .columnHasValue("Conversion or transfer date", nextMonthLong) // todo raise bug
            .columnHasValue("Project type", "Conversion")
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName) // not implemented
    });

    it("Should be able to view all projects by trust and all a trust's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By trust")
            .containsHeading("All projects by trust")
            .goToNextPageUntilFieldIsVisible(trust);
        projectTable.hasTableHeaders(["Trust", "Group identifier", "Conversions", "Transfers"]).filterBy(trust);
        allProjects
            // .containsHeading(`Projects for ${trust}`) // bug 208086
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            .columnHasValue("Conversion or transfer date", nextMonthLong)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view all projects by local authority and all a local authority's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By local authority")
            .containsHeading("All projects by local authority")
            .goToNextPageUntilFieldIsVisible(localAuthority);
        projectTable.hasTableHeaders(["Local authority", "Code", "Conversions", "Transfers"]).filterBy(localAuthority);
        allProjects.containsHeading(`Projects for ${localAuthority}`);
        projectTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type", "Assigned to"])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            // .columnHasValue("Conversion or transfer date", nextMonthLong) // bug 207849
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Assigned to", cypressUser.username)
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
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Team",
                "Type of project",
                "Conversion or transfer date",
                "Project completion date",
            ])
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });
});
