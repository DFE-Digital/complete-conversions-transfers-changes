import BasePage from "cypress/pages/basePage";

class LocalAuthorityPage extends BasePage {
    hasCode(code: string) {
        return this.assertSectionValue("Code", code);
    }

    hasAddressLines(addressLines: string[]) {
        addressLines.forEach((addressLine) => {
            this.assertSectionValue("Address", addressLine);
        });
        return this;
    }

    hasTown(town: string) {
        return this.assertSectionValue("Address", town);
    }

    hasCounty(county: string) {
        return this.assertSectionValue("Address", county);
    }

    hasPostcode(postcode: string) {
        return this.assertSectionValue("Address", postcode);
    }

    hasDCSPosition(position: string) {
        return this.assertSectionValue("DCS Position", position);
    }

    hasDCSName(name: string) {
        return this.assertSectionValue("Name", name);
    }

    hasDCSEmail(email: string) {
        return this.assertSectionValue("Email", email);
    }

    hasDCSPhone(phone: string) {
        return this.assertSectionValue("Phone", phone);
    }

    edit() {
        this.clickButton("Edit");
        return this;
    }

    delete() {
        cy.contains("a.govuk-button", "Delete").click();
        return this;
    }

    authorityCreatedSuccessMessage() {
        cy.getByClass("govuk-notification-banner--success").contains("Local authority successfully created");
        return this;
    }

    detailsUpdatedSuccessMessage() {
        cy.getByClass("govuk-notification-banner--success").contains("Local authority details updated");
        return this;
    }

    private assertSectionValue(section: string, value: string) {
        cy.contains(section).siblings().should("contain.text", value);
        return this;
    }
}

const localAuthorityPage = new LocalAuthorityPage();

export default localAuthorityPage;
