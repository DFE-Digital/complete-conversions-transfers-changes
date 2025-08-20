import ProjectTable from "cypress/pages/projects/tables/projectTable";

class GroupTable extends ProjectTable {
    withGroup(groupName: string) {
        return this.withSchool(groupName);
    }
}
const groupTable = new GroupTable();

export default groupTable;
