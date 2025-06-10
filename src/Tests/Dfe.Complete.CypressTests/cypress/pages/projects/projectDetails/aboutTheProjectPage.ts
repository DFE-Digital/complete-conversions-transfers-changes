import { ProjectDetailsPage } from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { significateDateToDisplayDate } from "cypress/support/formatDate";

class AboutTheProjectPage extends ProjectDetailsPage {
    private readonly giasUrl = "https://get-information-schools.service.gov.uk";

    hasProjectDetails(
        type: string,
        significantDate: string,
        localAuthority: string,
        diocese: string,
        region: string,
        groupRef: string,
    ) {
        cy.getById("projectDetails")
            .eq(1) // 2 projectDetails Ids issue
            .within(() => {
                this.containsSubHeading("Project details");
                this.keyHasValue("Type", type);
                if (type === "Transfer") {
                    this.keyHasValue("Transfer date", significateDateToDisplayDate(significantDate));
                } else {
                    this.keyHasValue("Conversion date", significateDateToDisplayDate(significantDate));
                }
                this.keyHasValue("Local authority", localAuthority);
                this.keyHasValue("Diocese", diocese);
                this.keyHasValue("Region", region);
                this.keyHasValue("Group reference number", groupRef);
                this.keyHasValueWithLink("Group reference number", groupRef, `/groups/`);
            });
        return this;
    }

    hasProjectAssignment(handingOverToRCS: boolean) {
        cy.getById("projectAssignment").within(() => {
            this.containsSubHeading("Project assignment");
            this.keyHasValue(
                "Are you handing this project over to RCS (Regional Casework Services)?",
                handingOverToRCS ? "Yes" : "No",
            );
        });
        return this;
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

    hasAdvisoryBoardDetails(advisoryBoardDate: string, advisoryBoardConditions: string) {
        cy.getById("advisoryBoardDetails").within(() => {
            this.containsSubHeading("Advisory board details");
            this.keyHasValue("Date of advisory board", significateDateToDisplayDate(advisoryBoardDate));
            this.keyHasValue("Conditions from advisory board", advisoryBoardConditions);
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
            this.keyHasValueWithLink("SharePoint folder", sharePointFolder, sharePointFolder);
        });
        return this;
    }

    hasIncomingTrustDetails(
        name: string,
        ukprn: number,
        groupId: string,
        companiesHouseNumber: string,
        address: string,
        sharePointFolder: string,
    ) {
        cy.getById("incomingTrustDetails").within(() => {
            this.containsSubHeading("Incoming trust details");
            this.keyHasValueWithLink(
                "Name",
                `${name} View the trust information in GIAS (opens in new tab)`,
                `${this.giasUrl}/Groups/Search?GroupSearchModel.Text=${ukprn}`,
            );
            this.keyHasValue("UKPRN (UK provider reference number)", ukprn);
            this.keyHasValue("Group ID (identifier)", groupId);
            this.keyHasValue("Companies House number", companiesHouseNumber);
            this.keyHasValue("Address", address);
            this.keyHasValueWithLink(
                "SharePoint folder",
                "View the trust SharePoint folder (opens in new tab)",
                sharePointFolder,
            );
        });
        return this;
    }

    hasOutgoingTrustDetails(
        name: string,
        ukprn: number,
        groupId: string,
        companiesHouseNumber: string,
        address: string,
        sharePointFolder: string,
        outGoingTrustWillClose: boolean,
    ) {
        cy.getById("outgoingTrustDetails").within(() => {
            this.containsSubHeading("Outgoing trust details");
            this.keyHasValueWithLink(
                "Name",
                `${name} View the trust information in GIAS (opens in new tab)`,
                `${this.giasUrl}/Groups/Search?GroupSearchModel.Text=${ukprn}`,
            );
            this.keyHasValue("UKPRN (UK provider reference number)", ukprn);
            this.keyHasValue("Group ID (identifier)", groupId);
            this.keyHasValue("Companies House number", companiesHouseNumber);
            this.keyHasValue("Address", address);
            this.keyHasValueWithLink(
                "SharePoint folder",
                "View the trust SharePoint folder (opens in new tab)",
                sharePointFolder,
            );
            this.keyHasValue(
                "Will the outgoing trust close once this transfer is completed?",
                outGoingTrustWillClose ? "Yes" : "No",
            );
        });
        return this;
    }
}

const aboutTheProjectPage = new AboutTheProjectPage();

export default aboutTheProjectPage;
