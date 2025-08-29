import BasePage from "cypress/pages/basePage";

class ProjectTable extends BasePage {
    protected readonly tableClass = "govuk-table";
    protected readonly tableData = "govuk-table__body";
    private readonly tableHeadClass = "govuk-table__head";
    private readonly tableHeadersClass = "govuk-table__header";
    private schoolName = "";

    withSchool(schoolName: string) {
        this.schoolName = schoolName;
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

    tableHasTableHeaders(table: string, headers: string[]) {
        cy.getById(table).within(() => {
            this.hasTableHeaders(headers);
        });
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
        this.assertTableCellValue(this.schoolName, tableColumn, expectedValue);
        return this;
    }

    columnContainsValue(tableColumn: string, expectedValue: string) {
        this.assertTableCellValue(this.schoolName, tableColumn, expectedValue, false);
        return this;
    }

    columnHasValueWithLink(tableColumn: string, expectedValue: string, link: string) {
        this.assertTableCellValue(this.schoolName, tableColumn, expectedValue, true, link);
    }

    clickButtonInRow(schoolName: string, buttonName: string) {
        cy.getProjectTableRow(schoolName).contains(buttonName).click();
        return this;
    }

    protected assertTableCellValue(
        tableRowKey: string,
        tableColumn: string,
        expectedValue: string,
        exactMatch = true,
        link?: string,
    ) {
        cy.getByClass(this.tableHeadersClass)
            .contains(tableColumn)
            .then((header) => {
                const tableColumnIndex = Cypress.$(header).index();
                if (!tableRowKey) {
                    throw new Error("School name is not set. Call withSchool() before asserting table cell value.");
                }
                cy.getProjectTableRow(tableRowKey).then((row) => {
                    const cell = row.find("td").eq(tableColumnIndex);
                    const actualValue = cell.text().trim();

                    if (exactMatch) {
                        expect(actualValue).to.equal(expectedValue);
                    } else {
                        expect(actualValue).contains(expectedValue);
                    }

                    if (link) {
                        const linkElement = cell.find("a");
                        expect(linkElement).to.exist;
                        expect(linkElement.attr("href")).to.contain(link);
                    }
                });
            });
        return this;
    }
}

export default ProjectTable;

export const projectTable = new ProjectTable();
