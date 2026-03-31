import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class OnHold extends TaskPage {
    continue() {
        this.clickButton("Continue");
        return this;
    }

    confirmHoldText(schoolName: string) {
        this.contains(`You are about to put ${schoolName} on hold. This will prevent any changes being made to the project until you resume it.`);
        return this;
    }

    confirmResumeText(schoolName: string, projectType: 'conversion' | 'transfer') {
        this.contains(`You are about to resume ${schoolName}. This will allow you to make changes to the project.`);
        this.contains(`Check that the ${projectType} date is correct.`);
        return this;
    }
}

const onHoldPage = new OnHold();

export default onHoldPage;
