import { NotePage } from "cypress/pages/projects/projectDetails/notePage";

class TaskNote extends NotePage {
    protected getNoteDiv() {
        return cy.contains(`li[id*="${this.noteId}"]`, this.noteText).should("exist");
    }
}

const taskNote = new TaskNote();

export default taskNote;
