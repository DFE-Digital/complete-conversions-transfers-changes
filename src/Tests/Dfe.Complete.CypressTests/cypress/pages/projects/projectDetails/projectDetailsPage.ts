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

    // tags
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
