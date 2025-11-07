import { before } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { cypressUser, rdoLondonUser } from "cypress/constants/cypressConstants";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import yourTeamProjectsTable from "cypress/pages/projects/tables/yourTeamProjectsTable";
import { currentMonthShort, todayFormatted } from "cypress/constants/stringTestConstants";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { urnPool } from "cypress/constants/testUrns";

const team = "London";
const myLondonProject = ProjectBuilder.createConversionProjectRequest({
    provisionalConversionDate: "2026-04-01",
    urn: urnPool.listings.cityHighgate,
});
const myLondonSchoolName = "City of London Academy, Highgate Hill";
const teammatesLondonRegionProject = ProjectBuilder.createConversionProjectRequest({
    provisionalConversionDate: "2026-04-01",
    urn: urnPool.listings.stJohns,
});
let teammatesLondonRegionProjectId: string;
const teammatesLondonSchoolName = "St John's and St Clement's Church of England Primary School";
const handedOverProject = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.listings.cityIslington,
});
const handedOverSchoolName = "City of London Academy Islington";

describe("Regional delivery officer (London) user - View your team projects (projects with London region)", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(myLondonProject.urn);
        projectRemover.removeProjectIfItExists(teammatesLondonRegionProject.urn);
        projectRemover.removeProjectIfItExists(handedOverProject.urn);
        projectApi.createConversionProject(myLondonProject);
        projectApi
            .createConversionProject(teammatesLondonRegionProject, rdoLondonUser.email)
            .then((response) => (teammatesLondonRegionProjectId = response.value));
        projectApi.createTransferProject(handedOverProject, rdoLondonUser.email);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit("/projects/team/in-progress");
    });

    it("Should be able to view my (London) project in my team project listings in progress", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToNextPageUntilFieldIsVisible(myLondonSchoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Team",
                "Assigned to",
                "Project type",
                "Form a MAT project",
                "Conversion or transfer date",
            ])
            .withSchool(myLondonSchoolName)
            .columnHasValue("URN", `${myLondonProject.urn}`)
            .columnHasValue("Local authority", "Islington")
            .columnHasValue("Team", team)
            .columnHasValue("Assigned to", cypressUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(myLondonSchoolName);
        projectDetailsPage.containsHeading(myLondonSchoolName);
    });

    it("Should be able to view my teammate's project (who is in the same region) in my team project listings in progress", () => {
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToNextPageUntilFieldIsVisible(teammatesLondonSchoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Team",
                "Assigned to",
                "Project type",
                "Form a MAT project",
                "Conversion or transfer date",
            ])
            .withSchool(teammatesLondonSchoolName)
            .columnHasValue("URN", `${teammatesLondonRegionProject.urn}`)
            .columnHasValue("Local authority", "Southwark")
            .columnHasValue("Team", team)
            .columnHasValue("Assigned to", rdoLondonUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(teammatesLondonSchoolName);
        projectDetailsPage.containsHeading(teammatesLondonSchoolName);
    });

    it("Should be able to view my team projects that are new", () => {
        yourTeamProjects.filterProjects("New").containsHeading("Your team new projects");
        yourTeamProjectsTable
            // .schoolIsFirstInTable(teammatesLondonSchoolName) // race condition issue
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Created at date",
                "Team",
                "Assigned to",
                "Project type",
                "Conversion or transfer date",
            ])
            .withSchool(teammatesLondonSchoolName)
            .columnHasValue("URN", `${teammatesLondonRegionProject.urn}`)
            .columnHasValue("Created at date", currentMonthShort)
            .columnHasValue("Team", team)
            .columnHasValue("Assigned to", rdoLondonUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(teammatesLondonSchoolName);
        projectDetailsPage.containsHeading(teammatesLondonSchoolName);
    });

    it("Should be able to view my team projects by user and all a user's projects", () => {
        yourTeamProjects
            .filterProjects("By user")
            .containsHeading("Your team projects by user")
            .goToNextPageUntilFieldIsVisible(rdoLondonUser.username);
        yourTeamProjectsTable
            .hasTableHeaders(["User name", "Email", "Conversions", "Transfers"])
            .goTo(rdoLondonUser.username);
        yourTeamProjects.containsHeading(`Projects assigned to ${rdoLondonUser.username}`);
        yourTeamProjectsTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type"])
            .withSchool(teammatesLondonSchoolName)
            .columnHasValue("URN", `${teammatesLondonRegionProject.urn}`)
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .columnHasValue("Project type", "Conversion")
            .goTo(teammatesLondonSchoolName);
        projectDetailsPage.containsHeading(teammatesLondonSchoolName);
    });

    it("Should be able to view my team projects that are handed over", () => {
        yourTeamProjects
            .filterProjects("Handed over")
            .containsHeading("Handed over")
            .goToNextPageUntilFieldIsVisible(handedOverSchoolName);
        yourTeamProjectsTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type", "Assigned to"])
            .withSchool(handedOverSchoolName)
            .columnHasValue("URN", `${handedOverProject.urn}`)
            .columnHasValue("Conversion or transfer date", "Mar 2026")
            .columnHasValue("Project type", "Transfer")
            .columnContainsValue("Assigned to", "Not yet assigned")
            .goTo(handedOverSchoolName);
        projectDetailsPage.containsHeading(handedOverSchoolName);
    });

    it("Should be able to view my team projects that are completed", () => {
        projectApi.completeProject(teammatesLondonRegionProjectId);
        yourTeamProjects.filterProjects("Completed").containsHeading("Your team completed projects");
        yourTeamProjectsTable
            // .schoolIsFirstInTable(teammatesLondonSchoolName)
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Team",
                "Type of project",
                "Conversion or transfer date",
                "Project completion date",
            ])
            .withSchool(teammatesLondonSchoolName)
            .columnHasValue("URN", `${teammatesLondonRegionProject.urn}`)
            .columnHasValue("Local authority", "Southwark")
            .columnHasValue("Team", team)
            .columnHasValue("Type of project", "Conversion")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .columnHasValue("Project completion date", todayFormatted)
            .goTo(teammatesLondonSchoolName);
        projectDetailsPage.containsHeading(teammatesLondonSchoolName);
    });

    it("Should NOT be able to view unassigned projects", () => {
        yourTeamProjects.unableToViewFilter("Unassigned");
        cy.visit("/projects/team/unassigned").notAuthorisedToPerformAction();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
