import { before } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import { regionalCaseworkerTeamLeaderUser, regionalCaseworkerUser } from "cypress/constants/cypressConstants";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import yourTeamProjectsTable from "cypress/pages/projects/tables/yourTeamProjectsTable";
import { currentMonthShort } from "cypress/constants/stringTestConstants";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    significantDate: "2026-04-01",
    urn: { value: urnPool.regionalWorker.morda },
    userAdId: regionalCaseworkerUser.adId,
});
const schoolName = "Morda CofE Primary School";
const teammatesProject = ProjectBuilder.createConversionProjectRequest({
    significantDate: "2026-04-01",
    urn: { value: urnPool.regionalWorker.mountjoy },
    userAdId: regionalCaseworkerTeamLeaderUser.adId,
});
const teammatesSchoolName = "Mountjoy House School";

describe("Regional caseworker services user - View your team projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectRemover.removeProjectIfItExists(teammatesProject.urn.value);
        projectApi.createConversionProject(project, regionalCaseworkerUser.email);
        projectApi.createConversionProject(teammatesProject, regionalCaseworkerTeamLeaderUser.email);
    });

    beforeEach(() => {
        cy.login(regionalCaseworkerUser);
        cy.acceptCookies();
        cy.visit("/projects/team/in-progress");
    });

    it("Should be able to view my project in my team project listings in progress", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects.containsHeading("Your team projects in progress").goToNextPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Region",
                "Assigned to",
                "Project type",
                "Form a MAT project",
                "Conversion or transfer date",
            ])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            .columnHasValue("Local authority", "Shropshire")
            .columnHasValue("Region", "West Midlands")
            .columnHasValue("Assigned to", regionalCaseworkerUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(schoolName);
        // projectDetailsPage.containsHeading(schoolName); // not implemented
    });

    it("Should be able to view my teammate's project in my team project listings in progress", () => {
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToNextPageUntilFieldIsVisible(teammatesSchoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Region",
                "Assigned to",
                "Project type",
                "Form a MAT project",
                "Conversion or transfer date",
            ])
            .withSchool(teammatesSchoolName)
            .columnHasValue("URN", `${teammatesProject.urn.value}`)
            .columnHasValue("Local authority", "Kirklees")
            .columnHasValue("Region", "Yorkshire and the Humber")
            .columnHasValue("Assigned to", regionalCaseworkerTeamLeaderUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it("Should be able to view my team projects that are new", () => {
        yourTeamProjects.filterProjects("New").containsHeading("Your team new projects");
        yourTeamProjectsTable
            .schoolIsFirstInTable(teammatesSchoolName)
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Created at date",
                "Region",
                "Assigned to",
                "Project type",
                "Conversion or transfer date",
            ])
            .withSchool(teammatesSchoolName)
            .columnHasValue("URN", `${teammatesProject.urn.value}`)
            .columnHasValue("Created at date", currentMonthShort)
            .columnHasValue("Region", "Yorkshire and the Humber")
            .columnHasValue("Assigned to", regionalCaseworkerTeamLeaderUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it("Should be able to view my team projects by user and all a user's projects", () => {
        yourTeamProjects
            .filterProjects("By user")
            .containsHeading("Your team projects by user")
            .goToNextPageUntilFieldIsVisible(regionalCaseworkerTeamLeaderUser.username);
        yourTeamProjectsTable
            .hasTableHeaders(["User name", "Email", "Conversions", "Transfers"])
            .goTo(regionalCaseworkerTeamLeaderUser.username);
        yourTeamProjects.containsHeading(`Projects assigned to ${regionalCaseworkerTeamLeaderUser.username}`);
        yourTeamProjectsTable
            .hasTableHeaders(["School or academy", "URN", "Conversion or transfer date", "Project type"])
            .withSchool(teammatesSchoolName)
            .columnHasValue("URN", `${teammatesProject.urn.value}`)
            .columnHasValue("Conversion or transfer date", "Apr 2026")
            .columnHasValue("Project type", "Conversion")
            .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName); // not implemented
    });

    it("Should be able to view my team projects that are completed", () => {
        yourTeamProjects.filterProjects("Completed").containsHeading("Your team completed projects");
        projectTable
            // .schoolIsFirstInTable(teammatesSchoolName) // project completion not implemented
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Region",
                "Type of project",
                "Conversion or transfer date",
                "Project completion date",
            ]);
        // not implemented, unable to move project to completed
        // .withSchool(teammatesSchoolName)
        // .columnHasValue("URN", `${teammatesProject.urn.value}`)
        // .columnHasValue("Local authority", "Halton")
        // .columnHasValue("Region", "North West")
        // .columnHasValue("Type of project", "Conversion")
        // .columnHasValue("Conversion or transfer date", "Apr 2026")
        // .columnHasValue("Project completion date", currentMonthShort)
        // .goTo(teammatesSchoolName);
        // projectDetailsPage.containsHeading(teammatesSchoolName);
    });

    it("Should NOT be able to view handed my team projects that are handed over", () => {
        yourTeamProjects.unableToViewFilter("Handed over");
        // not implemented:
        // cy.visit("/projects/team/handed-over").notAuthorisedToPerformAction();
    });
});
