class ProjectTable {
    protected readonly tableClass = "govuk-table";
    private readonly tableHeadClass = "govuk-table__head";
    private readonly tableHeadersClass = "govuk-table__header";
    private schoolName: string = "school-name-not-set";

    withSchool(schoolName: string) {
        this.schoolName = schoolName;
        cy.contains(schoolName).scrollIntoView();
        return this;
    }

    contains(field: string) {
        cy.getByClass(this.tableClass).contains(field);
        return this;
    }

    filterBy(filter: string) {
        cy.getByClass(this.tableClass).contains(filter).click();
        return this;
    }

    goTo(link: string) {
        this.filterBy(link);
        return this;
    }

    goToUserProjects(userName: string) {
        cy.getByClass(this.tableClass).contains(userName).click();
        return this;
    }

    hasTableHeaders(headers: string[]) {
        headers.forEach((header, index) => {
            cy.getByClass(this.tableClass)
                .getByClass(this.tableHeadClass)
                .getByClass(this.tableHeadersClass)
                .eq(index)
                .contains(header);
        });
        return this;
    }

    columnHasValue(tableColumn: string, expectedValue: string) {
        this.assertTableCellValue(tableColumn, expectedValue);
        return this;
    }

    columnContainsValue(tableColumn: string, expectedValue: string) {
        this.assertTableCellValue(tableColumn, expectedValue, false);
        return this;
    }

    protected clickButtonInRow(schoolName: string, buttonName: string) {
        cy.getProjectTableRow(schoolName).contains(buttonName).click();
        return this;
    }

    private assertTableCellValue(tableColumn: string, expectedValue: string, exactMatch = true) {
        cy.getByClass(this.tableHeadersClass)
            .contains(tableColumn)
            .then((header) => {
                const tableColumnIndex = Cypress.$(header).index();
                cy.getProjectTableRow(this.schoolName).then((row) => {
                    const actualValue = row.find("td").eq(tableColumnIndex).text();
                    if (exactMatch) {
                        expect(actualValue).to.equal(expectedValue);
                    } else {
                        expect(actualValue).contains(expectedValue);
                    }
                });
            });
        return this;
    }
}

export default ProjectTable;

export const projectTable = new ProjectTable();
