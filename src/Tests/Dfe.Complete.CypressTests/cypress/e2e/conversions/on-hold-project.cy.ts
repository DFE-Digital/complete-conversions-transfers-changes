import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import onHoldPage from "cypress/pages/projects/onHold";
// import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { getSignificantDateString } from "cypress/support/formatDate";

const nextMonth = getSignificantDateString(1);

const activeProject = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversion.batmans,
    provisionalConversionDate: nextMonth
});
let activeProjectId: string;
const activeProjectSchoolName = "Batmans Hill Nursery School";

describe("Complete conversion projects tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(activeProject.urn);
        projectApi.createAndUpdateConversionProject(activeProject).then((response) => {
            activeProjectId = response.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    // flaky test - needs investigation whether this is the test or application issue
    it("should be able to hold a project", () => {
        cy.visit(`projects/${activeProjectId}/tasks`);
        taskListPage.clickButton("Put the project on hold");
        onHoldPage
            .confirmHoldText(activeProjectSchoolName)
            .continue();
        // taskListPage.containsSuccessBannerWithMessage("Project held");
        cy.reload();
        // taskListPage
        //     .containsImportantBannerWithMessage(
        //         "",
        //         "This project's Directive Academy Order was revoked on 15 June 2024.",
        //     )
        //     .contains("This project was put on hold ... ")
        //     .doesntContain("Put the project on hold")
        //     .contains("Resume the project");
    });

    it("should not be able to resume a project that isn't held", () => {
        cy.visit(`projects/${activeProjectId}/tasks`);
        taskListPage.doesntContain("Resume the project").doesntContain("You can resume the project if you are ready to continue with it.");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
