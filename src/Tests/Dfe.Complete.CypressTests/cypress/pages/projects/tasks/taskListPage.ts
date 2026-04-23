import basePage from "cypress/pages/basePage";

class TaskListPage extends basePage {
    public selectTask(taskName: string) {
        cy.contains(taskName).click();
        return this;
    }
    public hasTaskStatusNotStarted(taskName: string) {
        this.hasTaskStatus(taskName, "Not started");
        return this;
    }

    public hasTaskStatusNotApplicable(taskName: string) {
        this.hasTaskStatus(taskName, "Not applicable");
        return this;
    }

    public hasTaskStatusCompleted(taskName: string) {
        this.hasTaskStatus(taskName, "Completed");
        return this;
    }

    public hasTaskStatusInProgress(taskName: string) {
        this.hasTaskStatus(taskName, "In progress");
        return this;
    }

    public hasValidationBannerWith(description: string, errors?: string[]) {
        const banner = cy.getByClass(this.bannerClass)
            .contains("p", description);

        if (errors?.length) {
            banner
                .parent()
                .parent()
                .within(() => {
                    cy.get("h2").should("contain.text", "Important");
                    cy.get("p").shouldHaveText(description);
                    for (const error of errors) {
                        cy.get("li").contains(error);
                    }
                    cy.get("li").should("have.length", errors?.length);
                });
        }
    }

    public hasImportantCompletedBannerWith(description: string, outstandingTasks: string[]) {
        this.hasValidationBannerWith(description, outstandingTasks);
    }

    private hasTaskStatus(taskName: string, status: string) {
        cy.contains(taskName).parents("li.app-task-list__item").find("strong").shouldHaveText(status);
        return this;
    }
}

const taskListPage = new TaskListPage();
export default taskListPage;
