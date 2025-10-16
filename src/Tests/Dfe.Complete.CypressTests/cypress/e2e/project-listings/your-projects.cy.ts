import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import yourProjects from "cypress/pages/projects/yourProjects";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { dimensionsTrust, macclesfieldTrust, todayFormatted } from "cypress/constants/stringTestConstants";
import { cypressUser } from "cypress/constants/cypressConstants";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { urnPool } from "cypress/constants/testUrns";

const conversionProject = ProjectBuilder.createConversionProjectRequest({
    significantDate: "2026-04-01",
    urn: { value: urnPool.listings.heles },
});
const conversionSchoolName = "Hele's School";
const transferProject = ProjectBuilder.createTransferProjectRequest({ urn: { value: urnPool.listings.queen } });
let transferProjectId: string;
const transferSchoolName = "Queen Elizabeth Grammar School Penrith";
const conversionFormAMatProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: urnPool.listings.themount },
});
const conversionFormAMatSchoolName = "The Mount School";
const transferFormAMatProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: { value: urnPool.listings.myddle },
});
const transferFormAMatSchoolName = "Myddle CofE Primary School";

describe("View your projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(conversionProject.urn.value);
        projectRemover.removeProjectIfItExists(transferProject.urn.value);
        projectRemover.removeProjectIfItExists(conversionFormAMatProject.urn.value);
        projectRemover.removeProjectIfItExists(transferFormAMatProject.urn.value);
        projectApi.createConversionProject(conversionProject);
        projectApi.createTransferProject(transferProject).then((response) => (transferProjectId = response.value));
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
            .columnHasValue("Local authority", "Plymouth")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(conversionSchoolName);
        projectDetailsPage.containsHeading(conversionSchoolName);
    });

    it("Should be able to view newly created transfer project in Your projects -> in progress", () => {
        yourProjects.goToNextPageUntilFieldIsVisible(transferSchoolName);
        projectTable
            .withSchool(transferSchoolName)
            .columnHasValue("URN", `${transferProject.urn.value}`)
            .columnHasValue("Type of project", "Transfer")
            .columnHasValue("Form a MAT project", "No")
            .columnContainsValue("Incoming trust", dimensionsTrust.name.toUpperCase()) // bug 208086
            .columnContainsValue("Outgoing trust", macclesfieldTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("Local authority", "Westmorland and Furness")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .goTo(transferSchoolName);
        projectDetailsPage.containsHeading(transferSchoolName);
    });

    it("Should be able to view newly created conversion form a MAT project in Your projects -> in progress", () => {
        yourProjects.goToNextPageUntilFieldIsVisible(conversionFormAMatSchoolName);
        projectTable
            .withSchool(conversionFormAMatSchoolName)
            .columnHasValue("URN", `${conversionFormAMatProject.urn.value}`)
            .columnHasValue("Type of project", "Conversion")
            .columnHasValue("Form a MAT project", "Yes")
            .columnContainsValue("Incoming trust", macclesfieldTrust.name)
            .columnContainsValue("Outgoing trust", "None")
            .columnHasValue("Local authority", "Kirklees")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .goTo(conversionFormAMatSchoolName);
        projectDetailsPage.containsHeading(conversionFormAMatSchoolName);
    });

    it("Should be able to view newly created transfer form a MAT project in Your projects -> in progress", () => {
        yourProjects.goToNextPageUntilFieldIsVisible(transferFormAMatSchoolName);
        projectTable
            .withSchool(transferFormAMatSchoolName)
            .columnHasValue("URN", `${transferFormAMatProject.urn.value}`)
            .columnHasValue("Type of project", "Transfer")
            .columnHasValue("Form a MAT project", "Yes")
            .columnContainsValue("Incoming trust", dimensionsTrust.name)
            .columnContainsValue("Outgoing trust", macclesfieldTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("Local authority", "Shropshire")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .goTo(transferFormAMatSchoolName);
        projectDetailsPage.containsHeading(transferFormAMatSchoolName);
    });

    it("Should be able to view newly created conversion project in Your projects -> added by you", () => {
        yourProjects
            .filterProjects("Added by you")
            .containsHeading("Added by you")
            .goToNextPageUntilFieldIsVisible(conversionSchoolName);
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

    it("Should be able to view completed transfer project in Your projects -> Completed", () => {
        projectApi.completeProject(transferProjectId);
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
            .columnHasValue("Local authority", "Westmorland and Furness")
            .columnHasValue("Team", "London")
            .columnHasValue("Type of project", "Transfer")
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .columnHasValue("Project completion date", todayFormatted)
            .goTo(transferSchoolName);
        projectDetailsPage.containsHeading(transferSchoolName);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
