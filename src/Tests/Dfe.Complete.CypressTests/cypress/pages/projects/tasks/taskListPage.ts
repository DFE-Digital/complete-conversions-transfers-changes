import basePage from "cypress/pages/basePage";

class TaskListPage extends basePage {
    public selectTask(taskName: string) {
        cy.contains(taskName).click();
        return this;
    }
    public hasTaskStatusNotStarted(taskName: string) {
        this.hasTaskStatus(taskName, "NOT STARTED");
        return this;
    }

    public hasTaskStatusNotApplicable(taskName: string) {
        this.hasTaskStatus(taskName, "NOT APPLICABLE");
        return this;
    }

    public hasTaskStatusCompleted(taskName: string) {
        this.hasTaskStatus(taskName, "COMPLETED");
        return this;
    }

    public hasTaskStatusInProgress(taskName: string) {
        this.hasTaskStatus(taskName, "IN PROGRESS");
        return this;
    }

    public hasImportantCompletedBannerWith(description: string, outstandingTasks: string[]) {
        cy.getByClass(this.bannerClass)
            .contains("p", description)
            .parent()
            .parent()
            .within(() => {
                cy.get("h2").should("contain.text", "Important");
                cy.get("p").shouldHaveText(description);
                for (const task of outstandingTasks) {
                    cy.get("li").contains(task);
                }
                cy.get("li").should("have.length", outstandingTasks.length);
            });
    }

    private hasTaskStatus(taskName: string, status: string) {
        cy.contains(taskName).parents("li.app-task-list__item").find("strong").shouldHaveText(status);
        return this;
    }
}

const taskListPage = new TaskListPage();
export default taskListPage;
