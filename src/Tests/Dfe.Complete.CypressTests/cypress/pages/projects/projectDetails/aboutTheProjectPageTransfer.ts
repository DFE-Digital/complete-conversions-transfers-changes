import { AboutTheProjectPage } from "cypress/pages/projects/projectDetails/aboutTheProjectPage";

class AboutTheProjectPageTransfer extends AboutTheProjectPage {
    hasProjectDetails(
        significantDate: string,
        localAuthority: string,
        diocese: string,
        region: string,
        groupRef: string,
    ) {
        return super.hasProjectDetails("Transfer", significantDate, localAuthority, diocese, region, groupRef);
    }

    hasReasonsForTheTransfer(isDueTo2Ri: boolean, isDueToInadequateOfstedRating: boolean, isDueToIssues: boolean) {
        cy.getById("reasonsForTransfer").within(() => {
            this.containsSubHeading("Reasons for the transfer");
            this.keyHasValue("Is this transfer due to 2RI?", isDueTo2Ri ? "Yes" : "No");
            this.keyHasValue(
                "Is this transfer due to an inadequate Ofsted rating?",
                isDueToInadequateOfstedRating ? "Yes" : "No",
            );
            this.keyHasValue(
                "Is this transfer due to financial, safeguarding or governance issues?",
                isDueToIssues ? "Yes" : "No",
            );
        });
        return this;
    }

    hasAcademyDetails(
        name: string,
        urn: number,
        type: string,
        ageRange: string,
        phase: string,
        sharePointFolder: string,
    ) {
        cy.getById("academyDetails").within(() => {
            this.containsSubHeading("Academy details");
            this.keyHasValueWithLink(
                "Name",
                `${name} View the school's information in GIAS (opens in new tab)`,
                `${this.giasUrl}/Establishments/Establishment/Details/${urn}`,
            );
            this.keyHasValue("Academy URN (unique reference number)", urn);
            this.keyHasValue("Type", type);
            this.keyHasValue("Age range", ageRange);
            this.keyHasValue("Phase", phase);
            this.keyHasValueWithLink(
                "SharePoint folder",
                "View the academy SharePoint folder (opens in new tab)",
                sharePointFolder,
            );
        });
        return this;
    }
}

const aboutTheProjectPageTransfer = new AboutTheProjectPageTransfer();

export default aboutTheProjectPageTransfer;
