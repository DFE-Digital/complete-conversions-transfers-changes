import LocalAuthority from "cypress/pages/service-support/localAuthority";

class NewLocalAuthorityPage extends LocalAuthority {
    withName(name: string) {
        cy.getById(this.nameId).typeFast(name);
        return this;
    }

    withCode(code: string) {
        cy.getById(this.codeId).typeFast(code);
        return this;
    }

    withAddressLines(...lines: string[]) {
        if (lines[0]) cy.getById(this.address1Id).typeFast(lines[0]);
        if (lines[1]) cy.getById(this.address2Id).typeFast(lines[1]);
        if (lines[2]) cy.getById(this.address3Id).typeFast(lines[2]);
        return this;
    }

    withTown(town: string) {
        cy.getById(this.addressTownId).typeFast(town);
        return this;
    }

    withCounty(county: string) {
        cy.getById(this.addressCountyId).typeFast(county);
        return this;
    }

    withPostcode(postcode: string) {
        cy.getById(this.addressPostcodeId).typeFast(postcode);
        return this;
    }

    withDCSPosition(dcsPosition: string) {
        cy.getById(this.dcsPositionId).typeFast(dcsPosition);
        return this;
    }

    withDCSName(dcsName: string) {
        cy.getById(this.dcsNameId).typeFast(dcsName);
        return this;
    }

    withDCSEmail(dcsEmail: string) {
        cy.getById(this.dcsEmailId).typeFast(dcsEmail);
        return this;
    }

    withDCSPhone(dcsPhone: string) {
        cy.getById(this.dcsPhoneId).typeFast(dcsPhone);
        return this;
    }
}

const newLocalAuthorityPage = new NewLocalAuthorityPage();

export default newLocalAuthorityPage;
