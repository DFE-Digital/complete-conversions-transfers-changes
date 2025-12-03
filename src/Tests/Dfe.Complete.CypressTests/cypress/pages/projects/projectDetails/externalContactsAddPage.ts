import { ExternalContactsEditPage } from "cypress/pages/projects/projectDetails/externalContactsEditPage";

class ExternalContactsAddPage extends ExternalContactsEditPage {
    private readonly inputId = "SelectedExternalContactType";

    selectHeadteacher() {
        cy.getById(this.inputId).check();
        return this;
    }

    selectIncomingTrustCEO() {
        cy.getById(this.inputId + "-2").check();
        return this;
    }

    selectChairOfGovernors() {
        cy.getById(this.inputId + "-3").check();
        return this;
    }

    selectSomeoneElse() {
        cy.getById(this.inputId + "-4").check();
        return this;
    }

    saveAndContinue() {
        this.clickButton("Save and continue");
        return this;
    }
}

const externalContactsAddPage = new ExternalContactsAddPage();

export default externalContactsAddPage;
