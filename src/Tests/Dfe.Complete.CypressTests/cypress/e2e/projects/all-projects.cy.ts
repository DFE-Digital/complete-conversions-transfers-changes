import navBar from "../../pages/navBar";
import allProjects from "../../pages/projects/allProjects";
import projectTable from "../../pages/projects/projectTable";
import {after, before} from "mocha";
import conversionProjectApi from "../../api/projectApi";
import {ProjectBuilder} from "../../api/projectBuilder";
import projectRemover from "../../api/projectRemover";

const project = ProjectBuilder.createConversionProjectRequest();
const schoolName = "Little Ilford School";
const userName = "Fahad Darwish";
describe("View all projects", () => {
    before(() => {
        conversionProjectApi.createProject(project);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.visit(`/projects/all/in-progress/all`);
    });

    after(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
    });

    it("Should be able to view newly created conversion project in All projects and Conversions projects", () => {
        navBar.goToAllProjects();
        allProjects
            .containsHeading("All projects in progress")
            .goToNextPageUntilProjectIsVisible(schoolName);
        projectTable
            .containsSchoolOrAcademy(schoolName)
            .schoolHasUrn(schoolName, `${project.urn}`)
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