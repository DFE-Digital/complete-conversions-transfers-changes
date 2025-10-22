import BasePage from "cypress/pages/basePage";
import { significateDateToDisplayDate } from "cypress/support/formatDate";

export class ProjectDetailsPage extends BasePage {
    protected readonly sectionId: string = "main-content";
    private readonly captionClass = "govuk-caption-l";
    private readonly navBarClass = "moj-sub-navigation";

    inOrder() {
        cy.wrap(this.sectionId).as("sectionId");
        cy.wrap(-1).as("summaryCounter");
        return this;
    }

    navigateTo(section: string) {
        const sectionUrlMap: Record<string, string> = {
            "Task list": "tasks",
            "About the project": "information",
            Notes: "notes",
            "External contacts": "external-contacts",
            "Internal contacts": "internal-contacts",
            "Conversion date history": "date-history",
            "Transfer date history": "date-history",
        };
        cy.getByClass(this.navBarClass).contains(section).click();
        cy.url().should("match", new RegExp(`\\/${sectionUrlMap[section]}$`));
        return this;
    }

    hasAcademyURNHeading(urn: string) {
        cy.getByClass(this.captionClass).contains(`Academy URN ${urn}`);
        return this;
    }

    hasSchoolURNHeading(urn: string) {
        cy.getByClass(this.captionClass).contains(`School URN ${urn}`);
        return this;
    }

    change(key: string) {
        cy.contains("dt", key).next("dd").next("dd").contains("Change").click();
        return this;
    }

    // tags
    hasConversionTag() {
        return this.hasTag("Conversion");
    }

    hasTransferTag() {
        return this.hasTag("Transfer");
    }

    hasFormAMATTag() {
        return this.hasTag("Form a MAT");
    }

    hasInAGroupTag() {
        return this.hasTag("In a group");
    }

    hasProvisionalDateTag() {
        cy.contains("dt", /^(Conversion|Transfer) date$/).next("dd").contains("provisional");
    }

    doesntHaveProvisionalDateTag() {
        cy.contains("dt", /^(Conversion|Transfer) date$/).next("dd").should("not.contain.text", "provisional");
    }

    // data

    hasConversionDate(significantDate: string) {
        return this.keyHasValue("Conversion date", significateDateToDisplayDate(significantDate));
    }

    hasTransferDate(significantDate: string) {
        return this.keyHasValue("Transfer date", significateDateToDisplayDate(significantDate));
    }

    hasOutgoingTrust(trustName: string) {
        return this.keyHasValue("Outgoing trust", trustName);
    }

    hasIncomingTrust(trustName: string) {
        return this.keyHasValue("Incoming trust", trustName);
    }

    hasLAAndRegion(laName: string, regionName: string) {
        this.keyHasValue("LA and region", `${laName}, ${regionName}`);
        return this;
    }

    hasSharePointLink(link: string) {
        return this.keyHasValueWithLink("SharePoint link", "Academy folder (opens in new tab)", link);
    }

    hasSharePointLinks(schoolLink: string, trustLink: string) {
        cy.contains("dt", "SharePoint links")
            .next("dd")
            .within(() => {
                cy.get("a").should("have.length", 2);
                cy.get("a")
                    .eq(0)
                    .shouldHaveText("School folder (opens in new tab)")
                    .should("have.attr", "href", schoolLink);
                cy.get("a")
                    .eq(1)
                    .shouldHaveText("Trust folder (opens in new tab)")
                    .should("have.attr", "href", trustLink);
            });
        return this;
    }

    summaryShows(key: string): this {
        cy.get("@summaryCounter").then((counter) => {
            const nextIndex = Number(counter) + 1;
            cy.wrap(nextIndex).as("summaryCounter");
            cy.get("@sectionId").then((id) => {
                cy.getById(id.toString()).find(".govuk-summary-list__key").eq(nextIndex).scrollIntoView().shouldHaveText(key);
            });
        });
        return this;
    }

    hasValue(value: string | number): this {
        this.hasValueWithLink(value);
        return this;
    }

    containsValue(value: string | number): this {
        cy.get("@summaryCounter").then((counter) => {
            cy.get("@sectionId").then((id) => {
                cy.getById(id.toString())
                    .find(".govuk-summary-list__value")
                    .eq(Number(counter))
                    .should("contain.text", value);
            });
        });
        return this;
    }

    hasValueWithLink(value: string | number, link?: string): this {
        cy.get("@summaryCounter").then((counter) => {
            cy.get("@sectionId").then((id) => {
                cy.getById(id.toString())
                    .find(".govuk-summary-list__value")
                    .eq(Number(counter))
                    .shouldHaveText(value)
                    .then(($el) => {
                        if (link) {
                            cy.wrap($el).find("a").should("have.attr", "href", link);
                        }
                    });
            });
        });
        return this;
    }

    hasTextWithLink(text: string, link: string, position: number = -1): this {
        cy.get("@summaryCounter").then((counter) => {
            cy.get("@sectionId").then((id) => {
                cy.getById(id.toString())
                    .find(".govuk-summary-list__value")
                    .eq(Number(counter))
                    .next("dd")
                    .find("a")
                    .eq(position)
                    .should("contain.text", text)
                    .should("have.attr", "href", link);
            });
        });
        return this;
    }

    protected keyHasValue(key: string, value: string | number) {
        cy.contains("dt", key).next("dd").contains(value);
        return this;
    }

    protected keyHasValueWithLink(key: string, value: string | number, link: string) {
        cy.contains("dt", key)
            .next("dd")
            .within(() => {
                cy.contains(value);
                cy.get("a").should("have.attr", "href", link);
            });
        return this;
    }

    private hasTag(tagName: string) {
        cy.getByClass(this.captionClass).contains(tagName);
        return this;
    }
}

const projectDetailsPage = new ProjectDetailsPage();

export default projectDetailsPage;
