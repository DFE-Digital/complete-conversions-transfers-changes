class ProjectTable {
    containsSchoolOrAcademy(schoolName: string) {
        cy.getByClass('govuk-table').contains(schoolName);
        return this;
    }

    containsUser(userName: string) {
        cy.getByClass('govuk-table').contains(userName);
        return this;
    }

    goToSchoolOrAcademy(schoolName: string) {
        cy.getByClass('govuk-table').contains(schoolName).click();
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

    private assertTableCellValue(schoolName: string, tableColumn: number, expectedValue: string) {
        cy.getProjectTableRow(schoolName).then((row) => {
            const actualValue = row.find('td').eq(tableColumn - 1).text();
            expect(actualValue).to.equal(expectedValue);
        });
    }

    schoolHasUrn(schoolName: string, expectedUrn: string) {
        this.assertTableCellValue(schoolName, 2, expectedUrn);
        return this;
    }

    schoolHasConversionOrTransferDate(schoolName: string, expectedDate: string) {
        this.assertTableCellValue(schoolName, 3, expectedDate);
        return this;
    }

    schoolHasProjectType(schoolName: string, expectedProjectType: string) {
        this.assertTableCellValue(schoolName, 4, expectedProjectType);
        return this;
    }

    schoolHasFormAMatProject(schoolName: string, expectedFormAMatProject: string) {
        this.assertTableCellValue(schoolName, 5, expectedFormAMatProject);
        return this;
    }

    schoolHasAssignedTo(schoolName: string, expectedAssignedTo: string) {
        this.assertTableCellValue(schoolName, 6, expectedAssignedTo);
        return this;
    }
}

const projectTable = new ProjectTable();

export default projectTable;