// Constants
const TASK_GROUPS = {
    PROJECT_KICKOFF: "Project kick-off",
    PROJECT_DETAILS: "Project details",
    LEGAL_DOCUMENTS: "Clear and sign legal documents",
    READY_FOR_OPENING: "Get ready for opening",
    AFTER_OPENING: "After opening",
} as const;

class ConversionTaskListPage {
    taskGroups = {
        projectKickOff: [
            TASK_GROUPS.PROJECT_KICKOFF,
            "Handover with regional delivery officer",
            "External stakeholder kick-off",
            "Confirm the academy's risk protection arrangements",
            "Check accuracy of high needs places information",
            "Complete a notification of changes to funded high needs places form",
            "Process conversion support grant",
            "Confirm and process the sponsored support grant",
            "Confirm the academy name",
            "Confirm the headteacher's details",
            "Confirm the chair of governors' details",
            "Confirm the incoming trust CEO's details",
            "Confirm the main contact",
            "Confirm the proposed capacity of the academy",
        ],
        legalDocuments: [
            "Land questionnaire",
            "Land registry title plans",
            "Supplemental funding agreement",
            "Church supplemental agreement",
            "Master funding agreement",
            "Articles of association",
            "Deed of variation",
            "Trust modification order",
            "Direction to transfer",
            "125 year lease",
            "Subleases",
            "Tenancy at will",
            "Commercial transfer agreement",
        ],
        readyForOpening: [
            "Confirm the school has completed all actions",
            "Confirm all conditions have been met",
            "Share the information about opening",
        ],
        afterOpening: [
            "Confirm the date the academy opened",
            "Redact and send documents",
            "Receive declaration of expenditure certificate",
        ],
    };

    visit(projectId: string) {
        cy.visit(`projects/${projectId}/tasks`);
        return this;
    }

    verifyTaskGroupExists(groupName: string) {
        cy.contains("h3", groupName).should("be.visible");
        return this;
    }

    verifyTaskExists(taskName: string) {
        cy.contains("a", taskName).should("be.visible");
        return this;
    }

    verifyAllTasksInGroup(tasks: string[]) {
        tasks.forEach((task) => {
            this.verifyTaskExists(task);
        });
        return this;
    }
}

const conversionTaskListPage = new ConversionTaskListPage();
export default conversionTaskListPage;
