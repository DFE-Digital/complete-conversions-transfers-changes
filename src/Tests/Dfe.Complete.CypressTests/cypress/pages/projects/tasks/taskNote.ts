import { NotePage } from "cypress/pages/projects/projectDetails/notePage";

class TaskNote extends NotePage {
    protected getNoteDiv() {
        return cy.contains("pre", this.noteText).should("exist").parent("li");
    }
}

const taskNote = new TaskNote();

export default taskNote;
