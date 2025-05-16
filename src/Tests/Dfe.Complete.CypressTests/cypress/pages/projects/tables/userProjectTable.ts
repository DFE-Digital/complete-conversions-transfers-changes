import ProjectTable from "cypress/pages/projects/tables/projectTable";

class UserProjectTable extends ProjectTable {
    private userName = "";

    withUser(userName: string) {
        this.userName = userName;
        return this;
    }

    columnHasValue(tableColumn: string, expectedValue: string) {
        super.assertTableCellValue(this.userName, tableColumn, expectedValue);
        return this;
    }
}

const userProjectTable = new UserProjectTable();

export default userProjectTable;
