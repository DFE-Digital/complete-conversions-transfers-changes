import Projects from "cypress/pages/projects/projects";

class ConversionURNsPage extends Projects {
    private readonly academyURNInputId = "URN";
    private readonly navBarClass = "moj-sub-navigation";

    enterAcademyURN(urn: number) {
        cy.getById(this.academyURNInputId).type(String(urn));
        return this;
    }

    viewURNsToCreate() {
        cy.getByClass(this.navBarClass).contains("URNs to create").click();
        return this;
    }

    viewURNsAdded() {
        cy.getByClass(this.navBarClass).contains("URNs added").click();
        return this;
    }

    hasLabel(label: string) {
        cy.get(`label[for="${this.academyURNInputId}"]`).should("contain.text", label);
        return this;
    }
}

const conversionURNsPage = new ConversionURNsPage();

export default conversionURNsPage;
