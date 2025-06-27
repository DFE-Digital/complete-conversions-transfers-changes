import ProjectTable from "./projectTable";

class YourTeamProjectsTable extends ProjectTable {
    schoolIsFirstInTable(schoolName: string) {
        cy.getByClass(this.tableClass).getByClass(this.tableData).find("tr").eq(0).should("contain", schoolName);
        return this;
    }

    assignProject(schoolName: string) {
        this.clickButtonInRow(schoolName, "Assign Project");
        return this;
    }
}

const yourTeamProjectsTable = new YourTeamProjectsTable();

export default yourTeamProjectsTable;
