import { ProjectBuilder } from "cypress/api/projectBuilder";
import { urnPool } from "cypress/constants/testUrns";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import daoRevocation from "cypress/pages/projects/daoRevocation";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const directiveAcademyOrderProject = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversion.batmans,
    directiveAcademyOrder: true,
});
let directiveAcademyOrderId: string;

const academyOrderProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversion.jessons,
    directiveAcademyOrder: false,
});
let academyOrderId: string;

describe("Complete conversion projects tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(directiveAcademyOrderProject.urn);
        projectRemover.removeProjectIfItExists(academyOrderProject.urn);
        projectApi.createAndUpdateConversionProject(directiveAcademyOrderProject).then((response) => {
            directiveAcademyOrderId = response.value;
        });
        projectApi.createAndUpdateMatConversionProject(academyOrderProject).then((response) => {
            academyOrderId = response.value;
        });
        // Intercept the POST requests to minister and date endpoints
        cy.intercept("POST", "**/dao-revocation/minister").as("addMinister");
        cy.intercept("POST", "**/dao-revocation/date").as("addDate");
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
    });

    // flaky test - needs investigation whether this is the test or application issue
    it.skip("should be able to revoke a directive academy order project assigned to me", () => {
        cy.visit(`projects/${directiveAcademyOrderId}/tasks`);
        taskListPage.clickButton("Record DAO revocation");
        daoRevocation
            .continue()
            .hasCheckboxLabel("I confirm a minister has approved this decision")
            .tick()
            .hasCheckboxLabel("I confirm I have sent the letter confirming the revocation decision")
            .tick()
            .hasCheckboxLabel("I confirm I have saved a copy of the letter to the school’s SharePoint folder")
            .tick()
            .continue()
            .selectReasonWithDetails(
                "School rated good or outstanding",
                "The school has improved its Ofsted rating to good.",
            )
            .selectReasonWithDetails(
                "Safeguarding concerns addressed",
                "The previous safeguarding issues have been resolved.",
            )
            .selectReasonWithDetails("School closed or closing", "The school has closed due to low enrollment.")
            .selectReasonWithDetails("Change to government policy", "New policies have made the order unnecessary.")
            .continue()
            .withMinisterName("Minister McMinisterface")
            .continue()
            .withDateOfDecision("15", "06", "2024")
            .continue();

        // Wait for the date POST request to complete
        cy.wait("@addMinister");
        cy.wait("@addDate");

        projectDetailsPage
            .inOrder()
            .summaryShows("Decision")
            .hasValue("DAO revoked")
            .summaryShows("Reasons")
            .containsValue("School rated good or outstanding")
            .containsValue("The school has improved its Ofsted rating to good.")
            .containsValue("Safeguarding concerns addressed")
            .containsValue("The previous safeguarding issues have been resolved.")
            .containsValue("School closed or closing")
            .containsValue("The school has closed due to low enrollment.")
            .containsValue("Change to government policy")
            .containsValue("New policies have made the order unnecessary.")
            .summaryShows("Decision maker's role")
            .hasValue("Minister")
            .summaryShows("Minister's name")
            .hasValue("Minister McMinisterface")
            .summaryShows("Date of decision")
            .hasValue("15 June 2024")
            .clickButton("Record DAO revocation");

        taskListPage.containsSuccessBannerWithMessage("DAO revocation recorded successfully");
        cy.reload();
        taskListPage
            .containsImportantBannerWithMessage(
                "",
                "This project's Directive Academy Order was revoked on 15 June 2024.",
            )
            .contains("Only Service Support team members can make changes to this project.")
            .doesntContain("Revoke a Directive Academy Order")
            .doesntContain("Record DAO revocation");
    });

    it("should not be able to revoke an academy order project", () => {
        cy.visit(`projects/${academyOrderId}/tasks`);
        taskListPage.doesntContain("Revoke a Directive Academy Order").doesntContain("Record DAO revocation");
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
