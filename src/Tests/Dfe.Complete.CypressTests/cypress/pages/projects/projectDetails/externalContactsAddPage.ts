import BasePage from "cypress/pages/basePage";
import { yesNoOption } from "cypress/constants/stringTestConstants";

class ExternalContactsAddPage extends BasePage {
    private readonly inputId = "SelectedExternalContactType";

    // contact type
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

    // contact details

    withName(name: string) {
        cy.getById("FullName").clear().type(name);
        return this;
    }

    withRole(role: string) {
        cy.getById("Role").clear().type(role);
        return this;
    }

    withEmail(email: string) {
        cy.getById("Email").clear().type(email);
        return this;
    }

    withPhone(phone: string) {
        cy.getById("Phone").clear().type(phone);
        return this;
    }

    withOrganisation(organisationType: string) {
        cy.contains("label", organisationType).prev().check();
        return this;
    }

    withPersonIsPrimaryContact(option: yesNoOption) {
        cy.enterYesNo("IsPrimaryProjectContact", option);
        return this;
    }

    saveAndContinue() {
        this.clickButton("Save and continue");
        return this;
    }
}

const externalContactsAddPage = new ExternalContactsAddPage();

export default externalContactsAddPage;
