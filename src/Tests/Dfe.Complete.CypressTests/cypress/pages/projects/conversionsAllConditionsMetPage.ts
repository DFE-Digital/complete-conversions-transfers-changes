import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class AllConditionsMetPage extends TaskPage {
    allConditionsMetSection = () => this.getFormGroupByLegend("Have all conditions been met?");
    shareInformationAboutOpeningSection = () => this.getFormGroupByLegend("Have you emailed your main contact the relevant information?");
}

const allConditionsMetPage = new AllConditionsMetPage();

export default allConditionsMetPage;