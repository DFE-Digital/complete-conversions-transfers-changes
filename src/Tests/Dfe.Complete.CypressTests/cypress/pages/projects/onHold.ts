import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class OnHold extends TaskPage {
    continue() {
        this.clickButton("Continue");
        return this;
    }

    confirmHoldText(schoolName: string) {
        this.contains(`You are about to put ${schoolName} on hold.`);
        return this;
    }

    confirmResumeText(schoolName: string, projectType: 'conversion' | 'transfer', significantDate: string) {
        this.contains(`You are about to resume ${schoolName}.`);
        this.contains(`The current ${projectType} date is: ${significantDate}`);
        this.contains(`Check that this date is correct when resuming the project.`);
        return this;
    }
}

const onHoldPage = new OnHold();

export default onHoldPage;
