import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class DaoRevocation extends TaskPage {
    continue() {
        this.clickButton("Continue");
        return this;
    }

    selectReasonWithDetails(reason: string, details: string) {
        cy.getById("reasons-hint").within(() => {
            cy.contains(reason).click().parent("div").next().find("textarea").typeFast(details);
        });
        return this;
    }

    withMinisterName(name: string) {
        cy.getById("minister-name").clear().typeFast(name);
        return this;
    }

    withDateOfDecision(day: string, month: string, year: string) {
        this.enterDate(day, month, year, "date");
        return this;
    }
}

const daoRevocationPage = new DaoRevocation();

export default daoRevocationPage;
