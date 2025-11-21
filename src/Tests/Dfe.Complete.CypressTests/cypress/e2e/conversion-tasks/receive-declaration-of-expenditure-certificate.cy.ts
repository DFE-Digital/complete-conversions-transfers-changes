import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import { ProjectType } from "cypress/api/taskApi";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import TaskHelperConversions from "cypress/api/taskHelperConversions";
import { getSignificantDateString } from "cypress/support/formatDate";
import receiveDeclarationOfExpenditureCertificateTaskPage from "cypress/pages/projects/tasks/receiveDeclarationOfExpenditureCertificateTaskPage";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.conversionTasks.spen,
});
let projectId: string;
let taskId: string;
const project2 = ProjectBuilder.createConversionFormAMatProjectRequest({
    provisionalConversionDate: getSignificantDateString(12),
    urn: urnPool.conversionTasks.huddersfield,
});
let project2Id: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: urnPool.conversionTasks.grylls,
});
let otherUserProjectId: string;

describe("Conversion tasks - Receive declaration of expenditure certificate", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectRemover.removeProjectIfItExists(project2.urn);
        projectRemover.removeProjectIfItExists(otherUserProject.urn);
        projectApi.createAndUpdateConversionProject(project).then((createResponse) => {
            projectId = createResponse.value;
            projectApi.getProject(project.urn).then((response) => {
                taskId = response.body.tasksDataId.value;
            });
        });
        projectApi.createAndUpdateMatConversionProject(project2).then((createResponse) => {
            project2Id = createResponse.value;
        });
        projectApi.createAndUpdateMatConversionProject(otherUserProject, rdoLondonUser).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/receive_grant_payment_certificate`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Update the grant assurance spreadsheet")
            .hasDropdownContent("Regional Casework Services team members must update the support grant assurance")
            .hasCheckboxLabel("Check the declaration of expenditure certificate is correct")
            .expandGuidance("Using the right certificate")
            .hasGuidance("If the school only received the £25,000 pre-opening support grant,");
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Select the Not applicable checkbox and save");
        taskPage.tickNotApplicable().saveAndReturn();
        taskListPage
            .hasTaskStatusNotApplicable("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");

        Logger.log("Unselect the Not applicable checkbox and save");
        taskPage.hasCheckboxLabel("Not applicable").isTicked().untick().saveAndReturn();
        taskListPage
            .hasTaskStatusNotStarted("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");
        taskPage.hasCheckboxLabel("Not applicable").isUnticked();
    });

    it("should show task status based on the checkboxes are checked", () => {
        cy.visit(`projects/${projectId}/tasks`);

        taskListPage.hasTaskStatusNotStarted("Receive declaration of expenditure certificate");

        TaskHelperConversions.updateReceiveDeclarationOfExpenditureCertificate(taskId, ProjectType.Conversion, "notApplicable");
        cy.reload();
        taskListPage.hasTaskStatusNotApplicable("Receive declaration of expenditure certificate");

        TaskHelperConversions.updateReceiveDeclarationOfExpenditureCertificate(taskId, ProjectType.Conversion, "inProgress");
        cy.reload();
        taskListPage.hasTaskStatusInProgress("Receive declaration of expenditure certificate");

        TaskHelperConversions.updateReceiveDeclarationOfExpenditureCertificate(taskId, ProjectType.Conversion, "completed");
        cy.reload();
        taskListPage.hasTaskStatusCompleted("Receive declaration of expenditure certificate");
    });

    it("Should only be able to confirm the received date of the declaration of expenditure certificate once", () => {
        cy.visit(`projects/${project2Id}/tasks/receive_grant_payment_certificate`);
        receiveDeclarationOfExpenditureCertificateTaskPage.enterDateReceived(10, 9, 2025).saveAndReturn();

        taskListPage
            .hasTaskStatusInProgress("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");
        receiveDeclarationOfExpenditureCertificateTaskPage
            .hasDate("10", "9", "2025")
            .enterDateReceived(11, 1, 2025)
            .saveAndReturn();

        taskListPage
            .hasTaskStatusInProgress("Receive declaration of expenditure certificate")
            .selectTask("Receive declaration of expenditure certificate");
        receiveDeclarationOfExpenditureCertificateTaskPage.hasDate("11", "1", "2025");
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/receive_grant_payment_certificate`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
