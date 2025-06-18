import { AboutTheProjectPage } from "cypress/pages/projects/projectDetails/aboutTheProjectPage";

class AboutTheProjectPageConversion extends AboutTheProjectPage {
    hasProjectDetails(
        significantDate: string,
        localAuthority: string,
        diocese: string,
        region: string,
        groupRef: string,
    ) {
        return super.hasProjectDetails("Conversion", significantDate, localAuthority, diocese, region, groupRef);
    }

    hasReasonsForTheConversion(hasAcademyOrderBeenIssued: boolean, isDueTo2Ri: boolean) {
        cy.getById("reasonsForConversion").within(() => {
            this.containsSubHeading("Reasons for the conversion");
            this.keyHasValue("Has a directive academy order been issued?", hasAcademyOrderBeenIssued ? "Yes" : "No");
            this.keyHasValue("Is this conversion due to intervention following 2RI?", isDueTo2Ri ? "Yes" : "No");
        });
        return this;
    }

    hasSchoolDetails(
        name: string,
        urn: number,
        type: string,
        ageRange: string,
        phase: string,
        sharePointFolder: string,
        address: string,
    ) {
        cy.getById("schoolDetails").within(() => {
            this.containsSubHeading("School details");
            this.keyHasValueWithLink(
                "Name",
                `${name} View the school's information in GIAS (opens in new tab)`,
                `${this.giasUrl}/Establishments/Establishment/Details/${urn}`,
            );
            this.keyHasValue("URN (unique reference number)", urn);
            this.keyHasValue("Type", type);
            this.keyHasValue("Range", ageRange);
            this.keyHasValue("Phase", phase);
            this.keyHasValue("Address", address);
            this.keyHasValueWithLink(
                "SharePoint folder",
                "View the school SharePoint folder (opens in new tab)",
                sharePointFolder,
            );
        });
        return this;
    }
}

const aboutTheProjectPageConversion = new AboutTheProjectPageConversion();

export default aboutTheProjectPageConversion;
