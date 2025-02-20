import navBar from "../../pages/navBar";
import allProjects from "../../pages/projects/allProjects";
import projectTable from "../../pages/projects/projectTable";
import {before} from "mocha";
import conversionProjectApi from "../../api/conversionProjectApi";
import {ProjectBuilder} from "../../api/projectBuilder";


const project = ProjectBuilder.createConversionProjectRequest();
const schoolName = "Little Ilford School";
describe("View all projects", () => {
    before(() => {
        conversionProjectApi.createProject(project);
    });

    beforeEach(() => {
        cy.login({ role: "RegionalDeliveryOfficer" });
        cy.visit(`/projects/all/in-progress/all`);
    });

    it("Should be able to view newly created conversion project in All projects and Conversions projects", () => {
        navBar.goToAllProjects();
        allProjects
            .containsAllProjectsInProgressHeader()
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

});