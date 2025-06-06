import LocalAuthority from "cypress/pages/service-support/localAuthority";

class EditLocalAuthorityPage extends LocalAuthority {
    hasCode(code: string) {
        cy.getById(this.codeId).should("have.value", code);
        return this;
    }

    hasAddressLine1(addressLine1: string) {
        cy.getById(this.address1Id).should("have.value", addressLine1);
        return this;
    }

    hasAddressLine2(addressLine2: string) {
        cy.getById(this.address2Id).should("have.value", addressLine2);
        return this;
    }

    hasAddressLine3(addressLine3: string) {
        cy.getById(this.address3Id).should("have.value", addressLine3);
        return this;
    }

    hasTown(town: string) {
        cy.getById(this.addressTownId).should("have.value", town);
        return this;
    }

    hasCounty(county: string) {
        cy.getById(this.addressCountyId).should("have.value", county);
        return this;
    }

    hasPostcode(postcode: string) {
        cy.getById(this.addressPostcodeId).should("have.value", postcode);
        return this;
    }

    hasDCSPosition(position: string) {
        cy.getById(this.dcsPositionId).should("have.value", position);
        return this;
    }

    hasDCSName(name: string) {
        cy.getById(this.dcsNameId).should("have.value", name);
        return this;
    }

    hasDCSEmail(email: string) {
        cy.getById(this.dcsEmailId).should("have.value", email);
        return this;
    }

    hasDCSPhone(phone: string) {
        cy.getById(this.dcsPhoneId).should("have.value", phone);
        return this;
    }

    editCode(code: string) {
        cy.getById(this.codeId).clear().typeFast(code);
        return this;
    }

    editAddressLine1(addressLine1: string) {
        cy.getById(this.address1Id).clear().typeFast(addressLine1);
        return this;
    }

    editAddressLine2(addressLine2: string) {
        cy.getById(this.address2Id).clear().typeFast(addressLine2);
        return this;
    }

    editAddressLine3(addressLine3: string) {
        cy.getById(this.address3Id).clear().typeFast(addressLine3);
        return this;
    }

    editTown(town: string) {
        cy.getById(this.addressTownId).clear().typeFast(town);
        return this;
    }

    editCounty(county: string) {
        cy.getById(this.addressCountyId).clear().typeFast(county);
        return this;
    }

    editPostcode(postcode: string) {
        cy.getById(this.addressPostcodeId).clear().typeFast(postcode);
        return this;
    }

    editDCSPosition(position: string) {
        cy.getById(this.dcsPositionId).clear().typeFast(position);
        return this;
    }

    editDCSName(name: string) {
        // id returns 2 elements
        cy.getById(this.dcsNameId).filter(":visible").clear().typeFast(name);
        return this;
    }

    editDCSEmail(email: string) {
        cy.getById(this.dcsEmailId).clear().typeFast(email);
        return this;
    }

    editDCSPhone(phone: string) {
        cy.getById(this.dcsPhoneId).clear().typeFast(phone);
        return this;
    }
}

const editLocalAuthorityPage = new EditLocalAuthorityPage();

export default editLocalAuthorityPage;
