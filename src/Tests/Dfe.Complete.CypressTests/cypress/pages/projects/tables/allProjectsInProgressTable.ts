import ProjectTable from "./projectTable";

class AllProjectsInProgressTable extends ProjectTable {
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

const allProjectsInProgressTable = new AllProjectsInProgressTable();

export default allProjectsInProgressTable;