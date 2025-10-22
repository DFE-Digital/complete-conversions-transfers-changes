import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class ReceiveDeclarationOfExpenditureCertificateTaskPage extends TaskPage {
    enterDateReceived(day: number, month: number, year: number) {
        cy.getById("received-date.Day").typeFast(String(day));
        cy.getById("received-date.Month").typeFast(String(month));
        cy.getById("received-date.Year").typeFast(String(year));
        return this;
    }

    hasDate(day: string, month: string, year: string): this {
        return super.hasDate(day, month, year, "received-date");
    }
}

const receiveDeclarationOfExpenditureCertificateTaskPage = new ReceiveDeclarationOfExpenditureCertificateTaskPage();

export default receiveDeclarationOfExpenditureCertificateTaskPage;
