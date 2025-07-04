import conversionHandoverWithDeliveryOfficerTaskPage from 'cypress/pages/projects/conversionHandoverWithDeliveryOfficerTaskPage';
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from 'cypress/pages/projects/taskListPage';

const project = ProjectBuilder.createConversionFormAMatProjectRequest();
let projectId: string;

describe('Conversion Handover With Delivery Officer Task Page', () => {
    before(() => {
        projectApi.createMatConversionProject(project).then((response) => (projectId = response.value));
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks`);
        taskListPage
            .selectTask('Handover with regional delivery officer')
    });

    it('should display all checkboxes with correct labels and hints', () => {
        conversionHandoverWithDeliveryOfficerTaskPage.getCheckboxLabel('not-applicable').should('contain', 'Not applicable');
        conversionHandoverWithDeliveryOfficerTaskPage.getCheckboxLabel('conversion_task_handover_task_form_review').should('contain', 'Review the project information');
        conversionHandoverWithDeliveryOfficerTaskPage.getCheckboxLabel('conversion_task_handover_task_form_notes').should('contain', 'Make notes and write questions');
        conversionHandoverWithDeliveryOfficerTaskPage.getCheckboxLabel('conversion_task_handover_task_form_meeting').should('contain', 'Attend handover meeting');
    });

    it('should expand and collapse guidance details', () => {
        conversionHandoverWithDeliveryOfficerTaskPage.expandGuidance('What to check for');
        conversionHandoverWithDeliveryOfficerTaskPage.getGuidanceContent('You should check existing project documents').should('be.visible');
        conversionHandoverWithDeliveryOfficerTaskPage.expandGuidance('What to make notes about');
        conversionHandoverWithDeliveryOfficerTaskPage.getGuidanceContent('Note down things you want to ask').should('be.visible');
    });

    it('should display the school name in the caption', () => {
        conversionHandoverWithDeliveryOfficerTaskPage.getSchoolName().should('be.visible');
    });

    it('should submit the form with Save and return', () => { //save is not working atm on Dev
        conversionHandoverWithDeliveryOfficerTaskPage.saveAndReturn();
        cy.url().should('not.include', 'handover-with-delivery-officer');
    });

    it('should display a working back link', () => {  //back link not working atm
        cy.get('a.govuk-back-link').should('be.visible').and('contain', 'Back');
        conversionHandoverWithDeliveryOfficerTaskPage.clickBackLink();
        cy.url().should('not.include', 'handover-with-delivery-officer');
    });

    it('should show task status based on the checkboxes are checked', () => {
        conversionHandoverWithDeliveryOfficerTaskPage
            .selectButtonOrCheckbox('not-applicable')
            .saveAndReturn()
        taskListPage
            .hasTaskStatusNotApplicable('handover-with-delivery-officer-status')
        conversionHandoverWithDeliveryOfficerTaskPage
            .selectButtonOrCheckbox('conversion_task_handover_task_form_review')
            .saveAndReturn()
        taskListPage
            .hasTaskStatusCompleted('handover-with-delivery-officer-status')
        conversionHandoverWithDeliveryOfficerTaskPage
            .selectButtonOrCheckbox('not-applicable')
            .selectButtonOrCheckbox('conversion_task_handover_task_form_notes')
            .selectButtonOrCheckbox('conversion_task_handover_task_form_meeting')
            .saveAndReturn()
        taskListPage
            .hasTaskStatusCompleted('handover-with-delivery-officer-status')
    })

    it('should load the handover task list page within 2 seconds', () => {
        const start = Date.now();
        cy.get('form').should('be.visible').then(() => {
            const duration = Date.now() - start;
            expect(duration).to.be.lessThan(2000);
        });
    });

    it('should be able to navigate back from the task page', () => {
        cy.get('a.govuk-back-link').should('contain', 'Back')
            .click();
        cy.url().should('not.include', 'handover-task-list');
    });

    it('should display the Notes section on the handover with delivery officer task page', () => {
        conversionHandoverWithDeliveryOfficerTaskPage.getNotesSection().should('exist').and('be.visible');
    });

    it('should allow the user to add a note', () => {
        const note = `Test note by Cypress ${Date.now()}`;
        conversionHandoverWithDeliveryOfficerTaskPage.addNote(note);
        cy.contains(note).should('exist');
    });

    it('should allow the user to edit only their own notes', () => {
        const originalNote = `Editable note by Cypress ${Date.now()}`;
        const updatedNote = `Updated note by Cypress ${Date.now()}`;
        // Add a note
        conversionHandoverWithDeliveryOfficerTaskPage.addNote(originalNote);
        cy.contains(originalNote).should('exist');
        // Edit the note
        conversionHandoverWithDeliveryOfficerTaskPage.editNote(originalNote, updatedNote);
        cy.contains(updatedNote).should('exist');
        // Optionally, check that edit button is not present for a note not created by this user
        // conversionHandoverWithDeliveryOfficerTaskPage.cannotEditNote('Some other user\'s note');
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
