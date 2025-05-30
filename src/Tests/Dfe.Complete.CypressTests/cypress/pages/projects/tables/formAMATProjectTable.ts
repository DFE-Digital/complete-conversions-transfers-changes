import ProjectTable from "cypress/pages/projects/tables/projectTable";

class FormAMATProjectTable extends ProjectTable {
    private Trust = "";

    withTrust(trustName: string) {
        this.Trust = trustName;
        return this;
    }

    columnHasValue(tableColumn: string, expectedValue: string) {
        super.assertTableCellValue(this.Trust, tableColumn, expectedValue);
        return this;
    }

    columnContainsValue(tableColumn: string, expectedValue: string) {
        super.assertTableCellValue(this.Trust, tableColumn, expectedValue, false);
        return this;
    }
}

const formAMATProjectTable = new FormAMATProjectTable();

export default formAMATProjectTable;
