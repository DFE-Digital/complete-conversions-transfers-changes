import ProjectTable from "./projectTable";

class YourTeamProjectsTable extends ProjectTable {
    schoolIsFirstInTable(schoolName: string) {
        cy.getByClass(super.tableClass).find("tr").eq(0).should("contain", schoolName);
        return this;
    }

    assignProject(schoolName: string) {
        this.clickButtonInRow(schoolName, "Assign project");
        return this;
    }
}

const yourTeamProjectsTable = new YourTeamProjectsTable();

export default yourTeamProjectsTable;
