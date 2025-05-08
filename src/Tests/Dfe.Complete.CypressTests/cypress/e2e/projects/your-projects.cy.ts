import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import yourProjects from "cypress/pages/projects/yourProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";

const project = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111394);
const schoolName = "Farnworth Church of England Controlled Primary School";

describe("View your projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectApi.createConversionProject(project);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`/projects/yours/in-progress`);
    });

    it("Should be able to view newly created conversion project in Your projects", () => {
        yourProjects.containsHeading("Your projects in progress").goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Type of project",
                "Form a MAT project",
                "Incoming trust",
                "Outgoing trust",
                "Local authority",
                "Conversion or transfer date",
            ])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            .columnHasValue("Type of project", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            // .columnHasValue("Incoming trust", trust) // bug 208086
            .columnContainsValue("Outgoing trust", "None")
            .columnHasValue("Local authority", "Halton")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    // awaiting BUG 201976 to be able to create the different project types
    it.skip("Should be able to view newly created transfer project in Your projects", () => {});
    it.skip("Should be able to view newly created conversion form a MAT project in Your projects", () => {});
    it.skip("Should be able to view newly created transfer form a MAT project in Your projects", () => {});
});
