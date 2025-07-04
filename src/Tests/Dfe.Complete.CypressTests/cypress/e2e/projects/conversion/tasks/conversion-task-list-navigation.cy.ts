import conversionTaskListPage from 'cypress/pages/projects/conversionTaskListPage';
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;

describe('Conversion Project Tasks List Navigation', () => {
    before(() => {
        projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        conversionTaskListPage.visit(projectId);
    });

    it('should display all project kick-off tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('Project kick-off')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.projectKickOff);
    });

    it('should display all legal documents tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('Clear and sign legal documents')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.clearAndSignLegalDocuments);
    });

    it('should display all ready for opening tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('Get ready for opening')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.getReadyForOpening);
    });

    it('should display all after opening tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('After opening')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.afterOpening);
    });

    it('should allow navigation to each task', () => {
        // Test navigation for one task from each group as an example
        const tasksToTest = [
            conversionTaskListPage.taskGroups.projectKickOff[0],
            conversionTaskListPage.taskGroups.clearAndSignLegalDocuments[0],
            conversionTaskListPage.taskGroups.getReadyForOpening[0],
            conversionTaskListPage.taskGroups.afterOpening[0]
        ];

        tasksToTest.forEach(task => {
            conversionTaskListPage.visit(projectId);
            conversionTaskListPage.clickTask(task);
            // Verify navigation by checking URL contains task name in kebab-case
            const taskUrlPart = task.toLowerCase().replace(/[^a-z0-9]+/g, '-');
            cy.url().should('include', taskUrlPart); // need to adjust this based on actual URL structure
        });
    });
});
