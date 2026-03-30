import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import onHoldPage from "cypress/pages/projects/onHold";
// import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { getSignificantDateString, toDisplayDate } from "cypress/support/formatDate";

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

    it("should be able to hold a project", () => {
        cy.visit(`projects/${activeProjectId}/tasks`);
        taskListPage
            .clickButton("Put the project on hold")
            .doesntContain("Resume the project")
            .doesntContain("You can resume the project if you are ready to continue with it.")

        onHoldPage
            .confirmHoldText(activeProjectSchoolName)
            .continue();

        taskListPage
            .containsSuccessBannerWithMessage("The project was put on hold.")
            .containsImportantBannerWithMessage("Project on hold", `The project was put on hold on ${toDisplayDate(new Date())}.`)
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
