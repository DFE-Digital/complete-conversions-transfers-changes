class AssignProject {
    assignTo(userName: string) {
        cy.getById("user-autocomplete").type(userName);
        cy.get("select").select(0).should("contain.text", userName);
        cy.getByClass("govuk-button").click();
        return this;
    }
}

const assignProject = new AssignProject();

export default assignProject;
