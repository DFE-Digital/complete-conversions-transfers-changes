import ProjectTable from "./projectTable";

class YourProjectsInProgressTable extends ProjectTable {

    schoolHasUrn(schoolName: string, expectedUrn: string) {
        this.assertTableCellValue(schoolName, 2, expectedUrn);
        return this;
    }

    schoolHasTypeOfProject(schoolName: string, expectedTypeOfProject: string) {
        this.assertTableCellValue(schoolName, 3, expectedTypeOfProject);
        return this;
    }

    schoolHasFormAMatProject(schoolName: string, expectedFormAMatProject: string) {
        this.assertTableCellValue(schoolName, 4, expectedFormAMatProject);
        return this;
    }

    schoolHasIncomingTrust(schoolName: string, expectedIncomingTrust: string) {
        this.assertTableCellContainsValue(schoolName, 5, expectedIncomingTrust);
        return this;
    }

    schoolHasOutgoingTrust(schoolName: string, expectedOutgoingTrust: string) {
        this.assertTableCellContainsValue(schoolName, 6, expectedOutgoingTrust);
        return this;
    }

    schoolHasLocalAuthority(schoolName: string, expectedLocalAuthority: string) {
        this.assertTableCellValue(schoolName, 7, expectedLocalAuthority);
        return this;
    }

    schoolHasConversionOrTransferDate(schoolName: string, expectedDate: string) {
        this.assertTableCellValue(schoolName, 8, expectedDate);
        return this;
    }
}

const yourProjectsInProgressTable = new YourProjectsInProgressTable();

export default yourProjectsInProgressTable;