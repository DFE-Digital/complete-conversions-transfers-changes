class TaskListPage {
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

    private hasTaskStatus(taskName: string, status: string) {
        cy.contains(taskName).parents("li.app-task-list__item").find("strong").shouldHaveText(status);
        return this;
    }
}

const taskListPage = new TaskListPage();
export default taskListPage;
