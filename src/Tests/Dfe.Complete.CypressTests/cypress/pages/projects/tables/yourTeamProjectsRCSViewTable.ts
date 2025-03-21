import YourTeamProjectsTable from "./yourTeamProjectsTable";

class YourTeamProjectsRDOViewTable extends YourTeamProjectsTable {
    schoolHasRegion(schoolName: string, expectedRegion: string) {
        this.assertTableCellValue(schoolName, 4, expectedRegion);
        return this;
    }
}

const yourTeamProjectsRDOViewTable = new YourTeamProjectsRDOViewTable();

export default yourTeamProjectsRDOViewTable;