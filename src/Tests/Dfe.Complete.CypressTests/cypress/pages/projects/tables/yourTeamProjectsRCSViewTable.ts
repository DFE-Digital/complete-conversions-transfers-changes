import YourTeamProjectsTable from "./yourTeamProjectsTable";

class YourTeamProjectsRCSViewTable extends YourTeamProjectsTable {
    schoolHasRegion(schoolName: string, expectedRegion: string) {
        this.assertTableCellValue(schoolName, 4, expectedRegion);
        return this;
    }
}

const yourTeamProjectsRCSViewTable = new YourTeamProjectsRCSViewTable();

export default yourTeamProjectsRCSViewTable;
