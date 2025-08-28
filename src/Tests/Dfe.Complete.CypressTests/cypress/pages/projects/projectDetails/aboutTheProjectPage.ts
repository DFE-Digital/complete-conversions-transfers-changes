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
    protected readonly sectionId = "projectInformationList";

    inOrder() {
        cy.wrap(this.sectionId).as("sectionId");
        cy.wrap(-1).as("summaryCounter");
        return this;
    }

    subSection(subSectionId: string) {
        cy.wrap(-1).as("summaryCounter");
        cy.wrap(this.sections[subSectionId]).as("sectionId");
        return this;
    }

    hasSubHeading(subHeading: string) {
        cy.get("@summaryCounter").then((counter) => {
            cy.get("@sectionId").then((id) => {
                cy.getById(id.toString()).find("h2").eq(Number(counter)).shouldHaveText(subHeading);
            });
        });
        return this;
    }

    hasChangeLink(linkPath: string) {
        return this.hasTextWithLink("Change", linkPath);
    }

    hasNoAcademyDetailsAsURNNotProvided() {
        cy.getById("academyDetails").within(() => {
            cy.contains("Academy URN has not been provided");
        });
        return this;
    }

    pageHasMovedToSection(section: string): this {
        return super.pageHasMovedToSection(section, this.sections);
    }

    keyHasValue(key: string, value: string | number) {
        cy.get("@sectionId").then((id) => {
            cy.getById(id.toString()).contains("dt", key).next("dd").shouldHaveText(value);
        });
        return this;
    }

    keyHasValueWithLink(key: string, value: string | number, link: string) {
        cy.get("@sectionId").then((id) => {
            cy.getById(id.toString())
                .contains("dt", key)
                .next("dd")
                .within(() => {
                    cy.contains(value);
                    cy.get("a").should("have.attr", "href", link);
                });
        });
        return this;
    }
}

const aboutTheProjectPage = new AboutTheProjectPage();

export default aboutTheProjectPage;
