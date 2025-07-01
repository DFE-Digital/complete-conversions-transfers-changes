import { ProjectDetailsPage } from "cypress/pages/projects/projectDetails/projectDetailsPage";

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

    constructor() {
        super("projectInformationList");
    }

    subSection(subSectionId: string) {
        cy.wrap(-1).as("summaryCounter");
        cy.wrap(this.sections[subSectionId]).as("subSectionId");
        return this;
    }

    hasSubHeading(subHeading: string) {
        cy.get("@summaryCounter").then((counter) => {
            cy.get("@subSectionId").then((subSectionId) => {
                cy.getById(subSectionId.toString()).find("h2").eq(Number(counter)).shouldHaveText(subHeading);
            });
        });
        return this;
    }

    hasChangeLink(linkPath: string) {
        return this.hasTextWithLink("Change", `${Cypress.config("baseUrl")}${linkPath}`);
    }

    hasNoAcademyDetailsAsURNNotProvided() {
        cy.getById("academyDetails").within(() => {
            cy.contains("Academy URN has not been provided");
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
