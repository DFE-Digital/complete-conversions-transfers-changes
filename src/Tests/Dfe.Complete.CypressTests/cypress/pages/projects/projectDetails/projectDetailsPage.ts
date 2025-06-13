import BasePage from "cypress/pages/basePage";
import { significateDateToDisplayDate } from "cypress/support/formatDate";

export class ProjectDetailsPage extends BasePage {
    private readonly captionClass = "govuk-caption-l";
    private readonly navBarClass = "moj-sub-navigation";

    navigateTo(section: string) {
        cy.getByClass(this.navBarClass).contains(section).click();
        cy.url().should("match", /\/information$/);
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
                    .should("have.text", "School folder (opens in new tab)")
                    .should("have.attr", "href", schoolLink);
                cy.get("a")
                    .eq(1)
                    .should("have.text", "Trust folder (opens in new tab)")
                    .should("have.attr", "href", trustLink);
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
