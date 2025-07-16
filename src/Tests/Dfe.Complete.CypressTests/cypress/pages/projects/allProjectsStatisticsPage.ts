import { currentMonthLong } from "cypress/constants/stringTestConstants";
import ProjectTable from "cypress/pages/projects/tables/projectTable";

class AllProjectsStatisticsPage extends ProjectTable {
    private readonly sections = {
        "Overview of all projects": "projectOverview",
        "Projects with Regional casework services": "projectsRCS",
        "Projects not with Regional casework services": "projectsNotRCS",
        "Conversion projects per region": "conversionProjectsPerRegion",
        "Transfer projects per region": "transferProjectsPerRegion",
        "6 month view of all project openers": "sixMonthView",
        [`New projects this month (${currentMonthLong})`]: "newProjects",
        "Users per team": "usersPerTeam",
    };

    pageHasMovedToSection(section: string): this {
        return super.pageHasMovedToSection(section, this.sections);
    }

    tableHasTableHeaders(tableSection: string, headers: string[]): this {
        cy.getById(this.sections[tableSection])
            .parent()
            .within(() => {
                this.hasTableHeaders(headers);
            });
        return this;
    }
}

const allProjectsStatisticsPage = new AllProjectsStatisticsPage();

export default allProjectsStatisticsPage;
