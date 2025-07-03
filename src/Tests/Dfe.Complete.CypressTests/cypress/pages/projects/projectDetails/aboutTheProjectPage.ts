import { ProjectDetailsPage } from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { significateDateToDisplayDate } from "cypress/support/formatDate";

export class AboutTheProjectPage extends ProjectDetailsPage {
    sections = {
        "Project details": "projectDetails",
        "Project assignment": "projectAssignment",
        "Reasons for the conversion": "reasonsForConversion",
        "Reasons for the transfer": "reasonsForTransfer",
        "Advisory board details": "advisoryBoardDetails",
        "School details": "schoolDetails",
        "Academy details": "academyDetails",
        "Incoming trust details": "incomingTrustDetails",
        "Outgoing trust details": "outgoingTrustDetails",
    };
    protected readonly giasUrl = "https://get-information-schools.service.gov.uk";

    hasProjectDetails(
        type: string,
        significantDate: string,
        localAuthority: string,
        diocese: string,
        region: string,
        groupRef: string,
    ) {
        cy.getById("projectDetails")
            .eq(-1) // 2 projectDetails Ids issue
            .within(() => {
                this.containsSubHeading("Project details");
                this.keyHasValue("Type", type);
                this.keyHasValue(`${type} date`, significateDateToDisplayDate(significantDate));
                this.keyHasValue("Local authority", localAuthority);
                this.keyHasValue("Diocese", diocese);
                this.keyHasValue("Region", region);
                this.keyHasValue("Group reference number", groupRef);
                if (groupRef === "Not grouped") {
                    this.keyHasValue("Group reference number", "Not grouped");
                } else {
                    this.keyHasValueWithLink("Group reference number", groupRef, `/groups/`);
                }
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

    hasAdvisoryBoardDetails(advisoryBoardDate: string, advisoryBoardConditions: string) {
        cy.getById("advisoryBoardDetails").within(() => {
            this.containsSubHeading("Advisory board details");
            this.keyHasValue("Date of advisory board", significateDateToDisplayDate(advisoryBoardDate));
            this.keyHasValue("Conditions from advisory board", advisoryBoardConditions);
        });
        return this;
    }

    hasNoAcademyDetailsAsURNNotProvided() {
        cy.getById("academyDetails").within(() => {
            cy.contains("Academy URN has not been provided");
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

    jumpToSection(section: string) {
        cy.contains("Jump to section")
            .parents("nav")
            .within(() => {
                cy.contains(section).click();
            });
        return this;
    }

    pageHasMovedToSection(section: string) {
        cy.url().should("include", `#${this.sections[section]}`);
        cy.contains("h2", section).isInViewport();
        return this;
    }
}

const aboutTheProjectPage = new AboutTheProjectPage();

export default aboutTheProjectPage;
