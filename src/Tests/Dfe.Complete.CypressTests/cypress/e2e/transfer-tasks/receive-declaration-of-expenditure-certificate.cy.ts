import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import projectRemover from "cypress/api/projectRemover";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperTransfers from "cypress/api/taskHelperTransfers";
import { getSignificantDateString } from "cypress/support/formatDate";
import receiveDeclarationOfExpenditureCertificateTaskPage from "cypress/pages/projects/tasks/receiveDeclarationOfExpenditureCertificateTaskPage";
import { urnPool } from "cypress/constants/testUrns";
import { rdoLondonUser } from "cypress/constants/cypressConstants";

const project = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.transferTasks.coquet,
});
let projectId: string;
const project2 = ProjectBuilder.createTransferFormAMatProjectRequest({
    provisionalTransferDate: getSignificantDateString(12),
    urn: urnPool.transferTasks.marden,
});
let project2Id: string;
let project2TaskId: string;
const otherUserProject = ProjectBuilder.createTransferFormAMatProjectRequest({
    urn: urnPool.transferTasks.whitley,
});
let otherUserProjectId: string;

describe("Transfers tasks - Receive declaration of expenditure certificate", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(project2.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateTransferProject(project).then((createResponse) => (projectId = createResponse.value));
        projectApi.createAndUpdateMatTransferProject(project2).then((createResponse) => {
            project2Id = createResponse.value;
            projectApi.getProject(project2.urn).then((response) => {
                project2TaskId = response.body.tasksDataId.value;
            });
        });
        projectApi.createAndUpdateMatTransferProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/declaration_of_expenditure_certificate`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("What to do if you have not received the certificate")
            .hasDropdownContent(
                "The incoming trust should send the certificate back to you within 10 days of the first reminder.",
            )
            .hasCheckboxLabel("Check the declaration of expenditure certificate is correct")
            .expandGuidance("How to check the declaration of expenditure certificate")
            .hasGuidance(
                "You can read guidance on how to process and check the declaration of expenditure certificate",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the 2 checkboxes, set date received and save");
        receiveDeclarationOfExpenditureCertificateTaskPage
            .hasCheckboxLabel("Check the declaration of expenditure certificate is correct")
            .tick()
            .hasCheckboxLabel("Save the declaration of expenditure certificate in the academy's SharePoint folder")
            .tick()
            .enterDateReceived(15, 7, 2025)
            .saveAndReturn();
        taskListPage
            .hasTaskStatusCompleted("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");

        Logger.log("Unselect the 2 checkboxes and save");
        receiveDeclarationOfExpenditureCertificateTaskPage
            .hasCheckboxLabel("Check the declaration of expenditure certificate is correct")
            .isTicked()
            .untick()
            .hasCheckboxLabel("Save the declaration of expenditure certificate in the academy's SharePoint folder")
            .isTicked()
            .untick()
            .hasDate("15", "7", "2025")
            .saveAndReturn();
        taskListPage
            .hasTaskStatusInProgress("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");
        receiveDeclarationOfExpenditureCertificateTaskPage
            .hasCheckboxLabel("Check the declaration of expenditure certificate is correct")
            .isUnticked()
            .hasCheckboxLabel("Save the declaration of expenditure certificate in the academy's SharePoint folder")
            .isUnticked()
            .hasDate("15", "7", "2025")
            .saveAndReturn();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${project2Id}/tasks`);

        TaskHelperTransfers.updateReceiveDeclarationOfExpenditureCertificate(project2TaskId, ProjectType.Transfer, "notStarted");
        taskListPage.hasTaskStatusNotStarted("Receive declaration of expenditure certificate");

        TaskHelperTransfers.updateReceiveDeclarationOfExpenditureCertificate(
            project2TaskId,
            ProjectType.Transfer,
            "notApplicable",
        );
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Receive declaration of expenditure certificate");

        TaskHelperTransfers.updateReceiveDeclarationOfExpenditureCertificate(project2TaskId, ProjectType.Transfer, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Receive declaration of expenditure certificate");

        TaskHelperTransfers.updateReceiveDeclarationOfExpenditureCertificate(project2TaskId, ProjectType.Transfer, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Receive declaration of expenditure certificate");
    });

    it("Should be able to update the date received multiple times", () => {
        cy.visit(`projects/${project2Id}/tasks/declaration_of_expenditure_certificate`);
        receiveDeclarationOfExpenditureCertificateTaskPage.enterDateReceived(10, 8, 2025).saveAndReturn();

        taskListPage
            .hasTaskStatusCompleted("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");
        receiveDeclarationOfExpenditureCertificateTaskPage
            .hasDate("10", "8", "2025")
            .enterDateReceived(11, 4, 2025)
            .saveAndReturn();

        taskListPage
            .hasTaskStatusCompleted("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");
        receiveDeclarationOfExpenditureCertificateTaskPage.hasDate("11", "4", "2025");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/declaration_of_expenditure_certificate`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
