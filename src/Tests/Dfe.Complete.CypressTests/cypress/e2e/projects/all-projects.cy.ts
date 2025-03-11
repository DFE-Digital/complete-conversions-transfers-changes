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
            .goToNextPageUntilProjectIsVisible(schoolName);
        projectTable
            .containsSchoolOrAcademy(schoolName)
            .schoolHasUrn(schoolName, `${project.urn.value}`)
            .schoolHasConversionOrTransferDate(schoolName, "Feb 2025")
            .schoolHasProjectType(schoolName, "Conversion")
            .schoolHasFormAMatProject(schoolName, "No")
            .schoolHasAssignedTo(schoolName, "Patrick Clark");
        allProjects
            .viewConversionsProjects()
            .goToNextPageUntilProjectIsVisible(schoolName);
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
            .containsUser(userName)
            .goToUserProjects(userName);
        allProjects.containsHeading(`Projects for ${userName}`);
        projectTable
            .hasTableHeader("School or academy")
            .hasTableHeader("URN")
            .hasTableHeader("Conversion or transfer date")
            .hasTableHeader("Project type")
            .containsSchoolOrAcademy(schoolName)
            .goToSchoolOrAcademy(schoolName);
        //project detail page
    })

});