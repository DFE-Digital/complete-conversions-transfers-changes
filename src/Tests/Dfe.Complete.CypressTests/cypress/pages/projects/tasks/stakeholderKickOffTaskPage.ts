import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class StakeholderKickOffTaskPage extends TaskPage {
    enterSignificantDate(month: number, year: number) {
        cy.getById("SignificantDate.Month").typeFast(String(month));
        cy.getById("SignificantDate.Year").typeFast(String(year));
        return this;
    }
}

const stakeholderKickOffTaskPage = new StakeholderKickOffTaskPage();

export default stakeholderKickOffTaskPage;