import BasePage from "cypress/pages/basePage";

class LocalAuthority extends BasePage {
    protected readonly nameId = "Name";
    protected readonly codeId = "Code";
    protected readonly address1Id = "Address1";
    protected readonly address2Id = "Address2";
    protected readonly address3Id = "Address3";
    protected readonly addressTownId = "AddressTown";
    protected readonly addressCountyId = "AddressCounty";
    protected readonly addressPostcodeId = "AddressPostcode";

    protected readonly dcsPositionId = "Title";
    protected readonly dcsNameId = "ContactName";
    protected readonly dcsEmailId = "Email";
    protected readonly dcsPhoneId = "Phone";

    saveAndReturn() {
        this.clickButton("Save and return");
        return this;
    }
}

export default LocalAuthority;
