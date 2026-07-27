import { TaskPage } from "cypress/pages/projects/tasks/taskPage";

class LandQuestionnairePage extends TaskPage {
    landQuestionnaireSection = () => this.getFormGroupByLegend("Check and clear the land questionnaire");
    landRegistrySection = () => this.getFormGroupByLegend("Check and clear the land registry title plans");
}

const landQuestionnairePage = new LandQuestionnairePage();

export default landQuestionnairePage;
