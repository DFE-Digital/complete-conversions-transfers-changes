import BasePage from "cypress/pages/basePage";
import { yesNoOption } from "cypress/constants/stringTestConstants";

export class ExternalContactsEditPage extends BasePage {
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
        cy.wrap(`organisation-${organisationType.toLowerCase()}`).as("organisationNameId");
        return this;
    }

    withOrganisationName(organisationName: string) {
        cy.get("@organisationNameId").then((id) => {
            cy.getById(String(id)).clear().type(organisationName);
        });
        return this;
    }

    withPersonIsPrimaryContact(option: yesNoOption) {
        cy.enterYesNo("IsPrimaryProjectContact", option);
        return this;
    }

    deleteContact() {
        this.clickButton("Delete");
        return this;
    }

    saveContact() {
        this.clickButton("Save contact");
        return this;
    }
}

const externalContactsEditPage = new ExternalContactsEditPage();

export default externalContactsEditPage;
