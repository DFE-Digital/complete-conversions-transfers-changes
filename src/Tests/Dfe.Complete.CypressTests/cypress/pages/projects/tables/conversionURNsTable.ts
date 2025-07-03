import ProjectTable from "cypress/pages/projects/tables/projectTable";

class ConversionURNsTable extends ProjectTable {
    createAcademyUrn(schoolName: string) {
        this.clickButtonInRow(schoolName, "Create academy URN");
        return this;
    }

    viewProject(schoolName: string) {
        this.clickButtonInRow(schoolName, "View project");
        return this;
    }
}

const conversionURNsTable = new ConversionURNsTable();

export default conversionURNsTable;
