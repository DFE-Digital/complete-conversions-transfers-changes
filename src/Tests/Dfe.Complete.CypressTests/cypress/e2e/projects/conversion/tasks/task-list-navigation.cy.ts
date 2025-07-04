import conversionTaskListPage from 'cypress/pages/projects/conversionTaskListPage';
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";

const TIMEOUT = {
    DEFAULT: 10000,
    EXTENDED: 15000
} as const;

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

    it('should display all Project kick-off tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('Project kick-off')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.projectKickOff);
    });

    it('should display all Clear and sign legal documents tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('Clear and sign legal documents')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.legalDocuments);
    });

    it('should display all Get ready for opening tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('Get ready for opening')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.readyForOpening);
    });

    it('should display all After opening tasks', () => {
        conversionTaskListPage
            .verifyTaskGroupExists('After opening')
            .verifyAllTasksInGroup(conversionTaskListPage.taskGroups.afterOpening);
    });

    it('should allow navigation to each task', () => {
        const sampleTasksForNavigation = [
            conversionTaskListPage.taskGroups.projectKickOff[0],
            conversionTaskListPage.taskGroups.projectDetails[0],
            conversionTaskListPage.taskGroups.legalDocuments[0],
            conversionTaskListPage.taskGroups.readyForOpening[0],
            conversionTaskListPage.taskGroups.afterOpening[0]
        ];

        sampleTasksForNavigation.forEach(task => {
            conversionTaskListPage.visit(projectId);
            conversionTaskListPage.clickTask(task);
            const taskUrlPart = task.toLowerCase().replace(/[^a-z0-9]+/g, '-');
            cy.url({ timeout: TIMEOUT.DEFAULT }).should('include', taskUrlPart);
        });
    });
});
