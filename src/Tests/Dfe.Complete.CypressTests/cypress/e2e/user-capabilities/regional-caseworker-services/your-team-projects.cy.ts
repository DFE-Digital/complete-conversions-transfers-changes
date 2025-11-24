import { before } from "mocha";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import navBar from "cypress/pages/navBar";
import yourTeamProjects from "cypress/pages/projects/yourTeamProjects";
import { regionalCaseworkerTeamLeaderUser, regionalCaseworkerUser } from "cypress/constants/cypressConstants";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import yourTeamProjectsTable from "cypress/pages/projects/tables/yourTeamProjectsTable";
import { currentMonthShort, todayFormatted } from "cypress/constants/stringTestConstants";
import { urnPool } from "cypress/constants/testUrns";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";

const project = ProjectBuilder.createConversionProjectRequest({
    provisionalConversionDate: "2028-04-01",
    urn: urnPool.userCapabilities.morda,
});
const schoolName = "Morda CofE Primary School";
const teammatesProject = ProjectBuilder.createConversionProjectRequest({
    provisionalConversionDate: "2028-04-01",
    urn: urnPool.userCapabilities.mountjoy,
});
let teammatesProjectId: string;
const teammatesSchoolName = "Mountjoy House School";

describe("Regional caseworker services user - View your team projects", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(teammatesProject.urn);
        projectApi.createConversionProject(project, regionalCaseworkerUser.email).then((response) => {
            projectApi.updateProjectHandoverAssign(
                ProjectBuilder.updateConversionProjectHandoverAssignRequest({
                    projectId: { value: response.value },
                    userId: { value: regionalCaseworkerUser.id },
                    userTeam: "RegionalCaseWorkerServices",
                }),
            );
        });
        projectApi
            .createAndUpdateConversionProject(teammatesProject, regionalCaseworkerTeamLeaderUser)
            .then((response) => {
                teammatesProjectId = response.value;
                projectApi.updateProjectHandoverAssign(
                    ProjectBuilder.updateConversionProjectHandoverAssignRequest({
                        projectId: { value: teammatesProjectId },
                        userId: { value: regionalCaseworkerTeamLeaderUser.id },
                        userTeam: "RegionalCaseWorkerServices",
                    }),
                );
            });
    });

    beforeEach(() => {
        cy.login(regionalCaseworkerUser);
        cy.acceptCookies();
        cy.visit("/projects/team/in-progress");
    });

    it("Should be able to view my project in my team project listings in progress", () => {
        navBar.goToYourTeamProjects();
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(schoolName);
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
            .columnHasValue("URN", `${project.urn}`)
            .columnHasValue("Local authority", "Shropshire")
            .columnHasValue("Region", "West Midlands")
            .columnHasValue("Assigned to", regionalCaseworkerUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            .columnHasValue("Conversion or transfer date", "Apr 2028")
            .goTo(schoolName);
    });

    it("Should be able to view my teammate's project in my team project listings in progress", () => {
        yourTeamProjects
            .containsHeading("Your team projects in progress")
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(teammatesSchoolName);
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
            .columnHasValue("URN", `${teammatesProject.urn}`)
            .columnHasValue("Local authority", "Kirklees")
            .columnHasValue("Region", "Yorkshire and the Humber")
            .columnHasValue("Assigned to", regionalCaseworkerTeamLeaderUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Form a MAT project", "No")
            .columnHasValue("Conversion or transfer date", "Apr 2028")
            .goTo(teammatesSchoolName);
        projectDetailsPage.containsHeading(teammatesSchoolName);
    });

    it("Should be able to view my team projects that are new", () => {
        yourTeamProjects.filterProjects("New").containsHeading("Your team new projects");
        yourTeamProjectsTable
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
            .columnHasValue("URN", `${teammatesProject.urn}`)
            .columnHasValue("Created at date", currentMonthShort)
            .columnHasValue("Region", "Yorkshire and the Humber")
            .columnHasValue("Assigned to", regionalCaseworkerTeamLeaderUser.username)
            .columnHasValue("Project type", "Conversion")
            .columnHasValue("Conversion or transfer date", "Apr 2028")
            .goTo(teammatesSchoolName);
        projectDetailsPage.containsHeading(teammatesSchoolName);
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
            .columnHasValue("URN", `${teammatesProject.urn}`)
            .columnHasValue("Conversion or transfer date", "Apr 2028")
            .columnHasValue("Project type", "Conversion")
            .goTo(teammatesSchoolName);
        projectDetailsPage.containsHeading(teammatesSchoolName);
    });

    it("Should be able to view my team projects that are completed", () => {
        projectApi.completeProject(teammatesProjectId);
        yourTeamProjects.filterProjects("Completed").containsHeading("Your team completed projects");
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Local authority",
                "Region",
                "Type of project",
                "Conversion or transfer date",
                "Project completion date",
            ])
            .withSchool(teammatesSchoolName)
            .columnHasValue("URN", `${teammatesProject.urn}`)
            .columnHasValue("Local authority", "Kirklees")
            .columnHasValue("Region", "Yorkshire and the Humber")
            .columnHasValue("Type of project", "Conversion")
            .columnHasValue("Conversion or transfer date", "Apr 2028")
            .columnHasValue("Project completion date", todayFormatted)
            .goTo(teammatesSchoolName);
        projectDetailsPage.containsHeading(teammatesSchoolName);
    });

    it("Should NOT be able to view your 'team projects that' are handed over", () => {
        yourTeamProjects.unableToViewFilter("Handed over");
        cy.visit("/projects/team/handed-over").notAuthorisedToPerformAction();
    });
});
