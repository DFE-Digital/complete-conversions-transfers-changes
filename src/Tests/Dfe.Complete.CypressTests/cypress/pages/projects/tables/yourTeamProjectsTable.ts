import ProjectTable from "./projectTable";

class YourTeamProjectsTable extends ProjectTable {
    schoolIsFirstInTable(schoolName: string) {
        cy.getByClass("govuk-table").find("tr").eq(0).should("contain", schoolName);
        return this;
    }

    schoolHasUrn(schoolName: string, expectedUrn: string) {
        this.assertTableCellValue(schoolName, 2, expectedUrn);
        return this;
    }

    schoolHasLocalAuthority(schoolName: string, expectedLocalAuthority: string) {
        this.assertTableCellValue(schoolName, 3, expectedLocalAuthority);
        return this;
    }

    schoolHasAssignedTo(schoolName: string, expectedAssignedTo: string) {
        this.assertTableCellValue(schoolName, 5, expectedAssignedTo);
        return this;
    }

    schoolHasProjectType(schoolName: string, expectedProjectType: string) {
        this.assertTableCellValue(schoolName, 6, expectedProjectType);
        return this;
    }

    schoolHasFormAMatProject(schoolName: string, expectedFormAMatProject: string) {
        this.assertTableCellValue(schoolName, 7, expectedFormAMatProject);
        return this;
    }

    schoolHasConversionOrTransferDate(schoolName: string, expectedDate: string) {
        this.assertTableCellValue(schoolName, 8, expectedDate);
        return this;
    }
}

export default YourTeamProjectsTable;
