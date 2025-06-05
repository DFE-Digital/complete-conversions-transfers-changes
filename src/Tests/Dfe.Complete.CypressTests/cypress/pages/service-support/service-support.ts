import BasePage from "cypress/pages/basePage";

class ServiceSupport extends BasePage {
    private readonly serviceSupportNavigation: string = "nav";

    viewConversionURNs() {
        cy.get(this.serviceSupportNavigation).contains("Conversion URNs").click();
        return this;
    }

    viewLocalAuthorities() {
        cy.get(this.serviceSupportNavigation).contains("Local authorities").click();
        return this;
    }

    viewUsers() {
        cy.get(this.serviceSupportNavigation).contains("Users").click();
        return this;
    }
}

const serviceSupport = new ServiceSupport();

export default serviceSupport;
