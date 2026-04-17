describe('LA Confirms Payroll Deadline Task', () => {
    beforeEach(() => {
        cy.login(); // Assuming a login command exists
        cy.visit('/projects/1/tasks/la_confirms_payroll_deadline');
    });

    it('should display the task page correctly', () => {
        cy.contains('The LA must confirm if they have a payroll deadline').should('be.visible');
        cy.get('[data-cy="payroll-deadline"]').should('exist');
    });

    it('should validate the date input', () => {
        cy.get('[data-cy="payroll-deadline-day"]').type('15');
        cy.get('[data-cy="payroll-deadline-month"]').type('04');
        cy.get('[data-cy="payroll-deadline-year"]').type('2025');
        cy.get('form').submit();
        cy.contains('The date must be in the future').should('be.visible');
    });

    it('should submit the form successfully', () => {
        cy.get('[data-cy="payroll-deadline-day"]').type('15');
        cy.get('[data-cy="payroll-deadline-month"]').type('04');
        cy.get('[data-cy="payroll-deadline-year"]').type('2027');
        cy.get('form').submit();
        cy.contains('Task completed successfully').should('be.visible');
    });
});