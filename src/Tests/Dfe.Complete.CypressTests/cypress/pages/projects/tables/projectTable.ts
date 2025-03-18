class ProjectTable {
    contains(field: string) {
        cy.getByClass('govuk-table').contains(field);
        return this;
    }

    filterBy(filter: string) {
        cy.getByClass('govuk-table').contains(filter).click();
        return this;
    }

    goTo(link: string) {
        this.filterBy(link);
        return this;
    }

    goToUserProjects(userName: string) {
        cy.getByClass('govuk-table').contains(userName).click();
        return this
    }

    hasTableHeader(header: string) {
        cy.getByClass('govuk-table')
            .getByClass("govuk-table__head")
            .contains(header);
        return this;
    }

    protected assertTableCellValue(schoolName: string, tableColumn: number, expectedValue: string) {
        cy.getProjectTableRow(schoolName).then((row) => {
            const actualValue = row.find('td').eq(tableColumn - 1).text();
            expect(actualValue).to.equal(expectedValue);
        });
    }

    protected assertTableCellContainsValue(schoolName: string, tableColumn: number, expectedValue: string) {
        cy.getProjectTableRow(schoolName).then((row) => {
            const actualValue = row.find('td').eq(tableColumn - 1).text();
            expect(actualValue).contains(expectedValue);
        });
    }

}
export default ProjectTable;

export const projectTable = new ProjectTable();