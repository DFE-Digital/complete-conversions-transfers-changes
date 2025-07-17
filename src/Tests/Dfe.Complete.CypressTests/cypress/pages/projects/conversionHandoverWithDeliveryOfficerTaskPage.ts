class ConversionHandoverWithDeliveryOfficerTaskPage {
    visit(projectId: string) {
        cy.visit(`projects/${projectId}/tasks/handover-with-delivery-officer`);
    }

    getSchoolName() {
        return cy.get('[data-testid="school-name"]');
    }

    getCheckbox(id: string) {
        return cy.get(`input[id="${id}"]`);
    }

    getCheckboxLabel(id: string) {
        return cy.get(`label[for="${id}"]`);
    }

    expandGuidance(summaryText: string) {
        cy.contains('summary', summaryText).click();
    }

    getGuidanceContent(content: string) {
        return cy.contains(content);
    }

    saveAndReturn() {
        cy.get('.govuk-button').contains('Save and return').click();
        cy.url().should('not.include', 'handover');
    }

    uncheckFirstCheckbox() {
    cy.get('input[type="checkbox"]').first().uncheck({ force: true });
  }

    clickBackLink() {
        cy.get('a.govuk-back-link').click();
    }

    public selectButtonOrCheckbox(id: string): this {
        cy.getById(id).click();

        return this;
    }

    // Notes section
    getNotesSection() {
        return cy.get('[data-testid="notes-section"], .notes-section, #notes-section'); // Adjust selector as needed
    }

    // Add note
    addNote(note: string) {
        cy.contains('Add a new task note').click();
        cy.get('textarea').type(note);
        cy.contains('Save').click();
    }

    // Edit note (by content)
    editNote(existingNote: string, updatedNote: string) {
        cy.contains(existingNote).parent().within(() => {
            cy.contains('Edit note').click();
        });
        cy.get('textarea').clear().type(updatedNote);
        cy.contains('Save').click();
    }

    // Check if edit button is present for a note
    canEditNote(note: string) {
        return cy.contains(note).parent().within(() => {
            cy.contains('Edit note').should('exist');
        });
    }

    // Check if edit button is not present for a note
    cannotEditNote(note: string) {
        return cy.contains(note).parent().within(() => {
            cy.contains('Edit note').should('not.exist');
        });
    }
}

const conversionHandoverWithDeliveryOfficerTaskPage = new ConversionHandoverWithDeliveryOfficerTaskPage();
export default conversionHandoverWithDeliveryOfficerTaskPage;
