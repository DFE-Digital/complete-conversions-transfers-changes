import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class ReceiveDeclarationOfExpenditureCertificateTaskPage extends TaskPage {
    enterDateReceived(day: number, month: number, year: number) {
        cy.getById("received-date.Day").typeFast(String(day));
        cy.getById("received-date.Month").typeFast(String(month));
        cy.getById("received-date.Year").typeFast(String(year));
        return this;
    }
}

const receiveDeclarationOfExpenditureCertificateTaskPage = new ReceiveDeclarationOfExpenditureCertificateTaskPage();

export default receiveDeclarationOfExpenditureCertificateTaskPage;
