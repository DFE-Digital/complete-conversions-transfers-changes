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
}

const onHoldPage = new OnHold();

export default onHoldPage;
