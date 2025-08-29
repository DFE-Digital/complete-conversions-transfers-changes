import BasePage from "cypress/pages/basePage";

class TaskPage extends BasePage {
    hasCheckboxLabel(text: string) {
        cy.contains("label", text).should("exist");
        cy.wrap(text).as("label");
        return this;
    }

    // input checkbox
    tickNotApplicable() {
        cy.getById("not-applicable").check();
        return this;
    }

    tick() {
        this.performLabelContainerAction(() => cy.get("input").check());
        return this;
    }

    untick() {
        this.performLabelContainerAction(() => cy.get("input").uncheck());
        return this;
    }

    isTicked() {
        this.performLabelContainerAction(() => cy.get("input").should("be.checked"));
        return this;
    }

    isUnticked() {
        this.performLabelContainerAction(() => cy.get("input").should("not.be.checked"));
        return this;
    }

    // checkbox validation

    expandGuidance(summaryText: string) {
        this.performLabelContainerAction(() => cy.contains("summary", summaryText).click());
        return this;
    }

    hasGuidance(content: string) {
        this.performLabelContainerAction(() => cy.contains(content).should("be.visible"));
        return this;
    }

    // save and return
    saveAndReturn() {
        this.clickButton("Save and return");
        return this;
    }

    noSaveAndReturnExists() {
        return this.buttonDoesNotExist("Save and return");
    }

    private performLabelContainerAction(action: () => void) {
        cy.get("@label").then((label) => {
            cy.contains("label", String(label))
                .parent()
                .within(() => {
                    action();
                });
        });
    }
}

const taskPage = new TaskPage();

export default taskPage;
