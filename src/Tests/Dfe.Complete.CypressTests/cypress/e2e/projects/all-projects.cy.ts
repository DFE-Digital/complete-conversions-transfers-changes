import navBar from "../../pages/navBar";
import allProjects from "../../pages/projects/allProjects";
import projectTable from "../../pages/projects/projectTable";
import {before, beforeEach} from "mocha";
import conversionProjectApi from "../../api/projectApi";
import {ProjectBuilder} from "../../api/projectBuilder";
import projectRemover from "../../api/projectRemover";

const project = ProjectBuilder.createConversionProjectRequest();
const schoolName = "St Chad's Catholic Primary School";
const userName = "Patrick Clark";
const region = "London";
const localAuthority = "Dudley Metropolitan Borough Council";
describe("View all projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        conversionProjectApi.createProject(project);
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
        projectTable
            .contains(schoolName)
            .schoolHasUrn(schoolName, `${project.urn.value}`)
            .schoolHasConversionOrTransferDate(schoolName, "Feb 2025")
            .schoolHasProjectType(schoolName, "Conversion")
            .schoolHasFormAMatProject(schoolName, "No")
            .schoolHasAssignedTo(schoolName, "Patrick Clark");
        allProjects
            .viewConversionsProjects()
            .goToNextPageUntilFieldIsVisible(schoolName);
    });

    it("Should be able to view all projects by User and all a user's projects", () => {
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