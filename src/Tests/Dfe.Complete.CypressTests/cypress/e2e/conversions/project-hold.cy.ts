import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import onHoldPage from "cypress/pages/projects/onHold";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { getSignificantDateString, toDisplayDate } from "cypress/support/formatDate";

const nextMonth = getSignificantDateString(1);

const activeProject = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversion.batmans,
    provisionalConversionDate: nextMonth
});
let activeProjectId: string;
const activeProjectSchoolName = "Batmans Hill Nursery School";

const heldProject = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversion.whitchurch,
    provisionalConversionDate: nextMonth
});
let heldProjectId: string;
const heldProjectSchoolName = "Whitchurch Primary School";

describe("Complete conversion projects tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(activeProject.urn);
        projectRemover.removeProjectIfItExists(heldProject.urn);

        projectApi.createAndUpdateConversionProject(activeProject).then((response) => {
            activeProjectId = response.value;
        });

        return projectApi.createAndUpdateConversionProject(heldProject).then((response) => {
            heldProjectId = response.value;
            return projectApi.holdProject(heldProjectId)
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    it("should be able to hold a project", () => {
        cy.visit(`projects/${activeProjectId}/tasks`);

        taskListPage
            .doesntContain("Resume the project")
            .doesntContain("You can resume the project if you are ready to continue with it.")
            .clickButton("Put the project on hold")

        onHoldPage
            .confirmHoldText(activeProjectSchoolName)
            .continue();

        taskListPage
            .containsSuccessBannerWithMessage("The project was put on hold.")
            .containsImportantBannerWithMessage("Project on hold", `The project was put on hold on ${toDisplayDate(new Date())}.`)
            .doesntContain("Put the project on hold")
            .doesntContain("You can put a project on hold if you need to pause the project for any reason.")
    });

    it("should be able to resume a project", () => {
        cy.visit(`projects/${heldProjectId}/tasks`);

        taskListPage
            .doesntContain("Put the project on hold")
            .doesntContain("You can put a project on hold if you need to pause the project for any reason.")
            .clickButton("Resume the project")

        onHoldPage
            .confirmResumeText(heldProjectSchoolName, 'conversion')
            .continue();

        taskListPage
            .containsSuccessBannerWithMessage("The project was resumed.")
            .doesntContain("Resume the project")
            .doesntContain("You can resume the project if you are ready to continue with it.")
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
