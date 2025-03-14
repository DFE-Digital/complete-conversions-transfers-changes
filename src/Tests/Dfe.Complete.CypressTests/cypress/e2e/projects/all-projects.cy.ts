import navBar from "../../pages/navBar";
import allProjects from "../../pages/projects/allProjects";
import { projectTable } from "../../pages/projects/tables/projectTable";
import {before, beforeEach} from "mocha";
import projectApi from "../../api/projectApi";
import {ProjectBuilder} from "../../api/projectBuilder";
import projectRemover from "../../api/projectRemover";
import allProjectsInProgressTable from "../../pages/projects/tables/allProjectsInProgressTable";

const project = ProjectBuilder.createConversionProjectRequest();
const schoolName = "St Chad's Catholic Primary School";
const userName = "Patrick Clark";
const region = "London";
const trust = "5 Dimensions Trust";
const localAuthority = "Dudley Metropolitan Borough Council";
const transferProject = ProjectBuilder.createTransferProjectRequest();
const transferSchoolName = "Newcastle Academy";
describe("View all projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${transferProject.urn.value}`);
        projectApi.createProject(project);
        projectApi.createProject(transferProject);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.acceptCookies();
        cy.visit(`/projects/all/in-progress/all`);
    });

    it("Should be able to view newly created conversion project in All projects and Conversions projects", () => {
        navBar.goToAllProjects();
        allProjects
            .containsHeading("All projects in progress")
            .goToNextPageUntilFieldIsVisible(schoolName);
        allProjectsInProgressTable
            .contains(schoolName)
            .schoolHasUrn(schoolName, `${project.urn.value}`)
            .schoolHasConversionOrTransferDate(schoolName, "Feb 2025")
            .schoolHasProjectType(schoolName, "Conversion")
            .schoolHasFormAMatProject(schoolName, "No")
            .schoolHasAssignedTo(schoolName, "Patrick Clark");
        allProjects
            .viewConversionsProjects()
            .containsHeading("All conversions in progress")
            .goToNextPageUntilFieldIsVisible(schoolName);
        projectTable.goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view newly created transfer project in All projects and Transfers projects", () => {
        navBar.goToAllProjects();
        allProjects.goToNextPageUntilFieldIsVisible(transferSchoolName);
        allProjectsInProgressTable
            .contains(transferSchoolName)
            .schoolHasUrn(transferSchoolName, `${transferProject.urn.value}`)
            .schoolHasConversionOrTransferDate(transferSchoolName, "Mar 2026")
            .schoolHasProjectType(transferSchoolName, "Transfer")
            .schoolHasFormAMatProject(transferSchoolName, "No")
            .schoolHasAssignedTo(transferSchoolName, "Patrick Clark");
        allProjects
            .viewTransfersProjects()
            .containsHeading("All transfers in progress")
            .goToNextPageUntilFieldIsVisible(transferSchoolName);
        projectTable.goTo(transferSchoolName);
        // projectDetailsPage.containsHeading(transferSchoolName); // not implemented
    });

    it("Should be able to view all Conversions projects by month", () => {
        const today = new Date();
        const nextMonth = new Date(today.setMonth(today.getMonth() + 1));
        const nextMonthString = `${nextMonth.toLocaleString('default', { month: 'long' })} ${nextMonth.getFullYear()}`;

        navBar.goToAllProjects();
        allProjects
            .filterProjects("By month")
            .containsHeading(nextMonthString)
        projectTable
            .hasTableHeader("School and URN")
            .hasTableHeader("Region")
            .hasTableHeader("Incoming trust")
            .hasTableHeader("All conditions met")
            .hasTableHeader("Confirmed date (Original date)")
            .contains(`${schoolName} ${project.urn.value}`)
            .goTo(`${schoolName} ${project.urn.value}`);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view all projects by region and all a region's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By region")
            .containsHeading("All projects by region")
        projectTable
            .hasTableHeader("Region")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .contains(region)
            .filterBy(region);
        // allProjects.containsHeading(`Projects for ${region}`); //bug 205144
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
        projectTable
            .hasTableHeader("User name")
            .hasTableHeader("Email")
            .hasTableHeader("Team")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .contains(userName)
            .goToUserProjects(userName);
        allProjects.containsHeading(`Projects for ${userName}`);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .contains(schoolName)
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName) // not implemented
    })

    it.skip("Should be able to view all projects by trust and all a trust's projects", () => {
        // not implemented 187140
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By trust")
            .containsHeading("All projects by trust");
        projectTable
            .hasTableHeader("Trust")
            .hasTableHeader("Group identifier")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers")
            .contains(trust)
            .filterBy(trust)
        allProjects.containsHeading(`Projects for ${trust}`);
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

    it("Should be able to view all projects by local authority and all a local authority's projects", () => {
        navBar.goToAllProjects();
        allProjects
            .filterProjects("By local authority")
            // .containsHeading("All projects by local authority"); // bug 205240
            .containsHeading("All projects by authority");
        projectTable
            .hasTableHeader("Local authority")
            .hasTableHeader("Code")
            .hasTableHeader("Conversions")
            .hasTableHeader("Transfers");
        allProjects.goToNextPageUntilFieldIsVisible(localAuthority);
        projectTable.filterBy(localAuthority);
        // allProjects.containsHeading(`Projects for ${localAuthority}`); // bug 205240
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

});