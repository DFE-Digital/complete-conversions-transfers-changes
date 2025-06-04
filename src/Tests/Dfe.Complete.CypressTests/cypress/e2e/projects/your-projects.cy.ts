import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import yourProjects from "cypress/pages/projects/yourProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { currentMonthShort, trust, trust2 } from "cypress/constants/stringTestConstants";
import { cypressUser } from "cypress/constants/cypressConstants";
import projectDetailsPage from "cypress/pages/projects/projectDetailsPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const conversionProject = ProjectBuilder.createConversionProjectRequest(new Date("2026-04-01"), 111394);
const conversionSchoolName = "Farnworth Church of England Controlled Primary School";
const transferProject = ProjectBuilder.createTransferProjectRequest();
const transferSchoolName = "Abbey College Manchester";
const conversionFormAMatProject = ProjectBuilder.createConversionFormAMatProjectRequest();
const conversionFormAMatSchoolName = "Whitchurch Primary School";
const transferFormAMatProject = ProjectBuilder.createTransferFormAMatProjectRequest();
const transferFormAMatSchoolName = "Priory Rise School";

describe("View your projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${conversionProject.urn.value}`);
        projectRemover.removeProjectIfItExists(`${transferProject.urn.value}`);
        projectRemover.removeProjectIfItExists(`${conversionFormAMatProject.urn.value}`);
        projectRemover.removeProjectIfItExists(`${transferFormAMatProject.urn.value}`);
        projectApi.createConversionProject(conversionProject);
        projectApi.createTransferProject(transferProject);
        projectApi.createMatConversionProject(conversionFormAMatProject);
        projectApi.createMatTransferProject(transferFormAMatProject);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`/projects/yours/in-progress`);
    });

    it("Should be able to view newly created conversion project in Your projects -> in progress", () => {
        yourProjects.containsHeading("Your projects in progress").goToNextPageUntilFieldIsVisible(conversionSchoolName);
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
            .withSchool(conversionSchoolName)
            .columnHasValue("URN", `${conversionProject.urn.value}`)
            .columnHasValue("Type of project", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            // .columnHasValue("Incoming trust", trust) // bug 208086
            .columnContainsValue("Outgoing trust", "None")
            .columnHasValue("Local authority", "Halton")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(conversionSchoolName);
        projectDetailsPage.containsHeading(conversionSchoolName);
    });

    it("Should be able to view newly created transfer project in Your projects -> in progress", () => {
        projectTable
            .withSchool(transferSchoolName)
            .columnHasValue("URN", `${transferProject.urn.value}`)
            .columnHasValue("Type of project", "Transfer")
            .columnHasValue("Form a MAT project", "No")
            .columnContainsValue("Incoming trust", trust2.toUpperCase()) // bug 208086
            .columnContainsValue("Outgoing trust", trust.toUpperCase()) // bug 208086
            .columnHasValue("Local authority", "Manchester")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .goTo(transferSchoolName);
        projectDetailsPage.containsHeading(transferSchoolName);
    });

    it("Should be able to view newly created conversion form a MAT project in Your projects -> in progress", () => {
        projectTable
            .withSchool(conversionFormAMatSchoolName)
            .columnHasValue("URN", `${conversionFormAMatProject.urn.value}`)
            .columnHasValue("Type of project", "Conversion")
            .columnHasValue("Form a MAT project", "Yes")
            // .columnContainsValue("Incoming trust", testTrustName) // bug 212413
            .columnContainsValue("Outgoing trust", "None")
            .columnHasValue("Local authority", "Bath and North East Somerset")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .goTo(conversionFormAMatSchoolName);
        projectDetailsPage.containsHeading(conversionFormAMatSchoolName);
    });

    it("Should be able to view newly created transfer form a MAT project in Your projects -> in progress", () => {
        projectTable
            .withSchool(transferFormAMatSchoolName)
            .columnHasValue("URN", `${transferFormAMatProject.urn.value}`)
            .columnHasValue("Type of project", "Transfer")
            .columnHasValue("Form a MAT project", "Yes")
            // .columnContainsValue("Incoming trust", testTrustName)  // bug 212413
            .columnContainsValue("Outgoing trust", trust.toUpperCase()) // bug 208086
            .columnHasValue("Local authority", "Milton Keynes")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .goTo(transferFormAMatSchoolName);
        projectDetailsPage.containsHeading(transferFormAMatSchoolName);
    });

    it("Should be able to view newly created conversion project in Your projects -> added by you", () => {
        yourProjects.filterProjects("Added by you").containsHeading("Added by you");
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Conversion or transfer date",
                "Project type",
                "Form a MAT project",
                "Assigned to",
            ])
            .withSchool(conversionSchoolName)
            .columnHasValue("URN", `${conversionProject.urn.value}`)
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            .columnHasValue("Assigned to", cypressUser.username)
            .goTo(conversionSchoolName);
        projectDetailsPage.containsHeading(conversionSchoolName);
    });

    it.skip("Should be able to view completed transfer project in Your projects -> Completed", () => {
        // project completion not implemented
        yourProjects.filterProjects("Completed").containsHeading("Completed");
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
            .withSchool(transferSchoolName)
            .columnHasValue("URN", `${transferProject.urn.value}`)
            .columnHasValue("Local authority", "Manchester")
            .columnHasValue("Team", "North West")
            .columnHasValue("Type of project", "Transfer")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .columnHasValue("Project completion date", currentMonthShort)
            .goTo(transferSchoolName);
        projectDetailsPage.containsHeading(transferSchoolName);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
