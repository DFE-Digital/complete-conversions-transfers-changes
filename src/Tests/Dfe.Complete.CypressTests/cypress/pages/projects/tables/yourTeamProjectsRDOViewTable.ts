import YourTeamProjectsTable from "./yourTeamProjectsTable";

class YourTeamProjectsRDOViewTable extends YourTeamProjectsTable {
    schoolHasTeam(schoolName: string, expectedTeam: string) {
        this.assertTableCellValue(schoolName, 4, expectedTeam);
        return this;
    }
}

const yourTeamProjectsRDOViewTable = new YourTeamProjectsRDOViewTable();

export default yourTeamProjectsRDOViewTable;
