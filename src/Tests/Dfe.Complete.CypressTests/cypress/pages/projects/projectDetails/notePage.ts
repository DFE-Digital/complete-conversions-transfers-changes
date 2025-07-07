import { ProjectDetailsPage } from "cypress/pages/projects/projectDetails/projectDetailsPage";

class NotePage extends ProjectDetailsPage {
    private readonly noteTextId = "note-text";
    private noteText = "";

    enterNote(noteText: string) {
        cy.getById(this.noteTextId).clear().type(noteText);
        return this;
    }

    noteTextboxHasValue(expectedValue: string) {
        cy.getById(this.noteTextId).should("have.value", expectedValue);
        return this;
    }

    withNote(noteText: string) {
        this.noteText = noteText;
        return this;
    }

    hasLabel(label: string) {
        cy.get(`label[for="note-text"]`).should("contain.text", label);
        return this;
    }

    hasDate(expectedDate: string) {
        this.getNoteDiv().should("contain.text", expectedDate);
        return this;
    }

    hasUser(userName: string) {
        this.getNoteDiv().should("contain.text", userName);
        return this;
    }

    editNote() {
        this.getNoteDiv().contains("Edit").click();
        return this;
    }

    noEditNoteLink() {
        this.getNoteDiv().contains("Edit").should("not.exist");
    }

    private getNoteDiv() {
        return cy.contains('div[id*="note-entry-NoteId"]', this.noteText).should("exist");
    }
}

const notePage = new NotePage();

export default notePage;
