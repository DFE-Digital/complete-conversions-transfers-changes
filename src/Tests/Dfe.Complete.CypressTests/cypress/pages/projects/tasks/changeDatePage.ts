import basePage from "cypress/pages/basePage";

class ChangeDatePage extends basePage {
    enterDate(month: number, year: number) {
        cy.getById("SignificantDate.Month").typeFast(String(month));
        cy.getById("SignificantDate.Year").typeFast(String(year));
        return this;
    }

    saveAndContinue() {
        this.clickButton("Save and continue");
        return this;
    }

    selectReasonWithDetails(reason: string, details: string) {
        cy.getByClass("govuk-checkboxes").within(() => {
            cy.contains(reason).click().parent("div").next().find("textarea").typeFast(details);
        });

        return this;
    }
}

const changeDatePage = new ChangeDatePage();

export default changeDatePage;
