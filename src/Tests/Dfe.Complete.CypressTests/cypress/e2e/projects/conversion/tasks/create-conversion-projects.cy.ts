import conversionProjectApi from "cypress/api/conversionProjectApi";
import { Logger } from "cypress/common/logger";
import editHandoverWithDeliveryOfficerPage from "cypress/pages/projects/tasks/editHandoverWithDeliveryOfficerPage";
import projectSummarySection from "cypress/pages/projects/projectSummarySection";
import taskListPage, { ConversionTaskNames } from "cypress/pages/projects/taskListPage";
import summaryPage from "cypress/pages/projects/summaryPage";
import { ProjectBuilder } from "cypress/api/projectBuilder";

describe("Create a new Conversion Project", () => {

    let projectId: string;

    beforeEach(() => {
        cy.login();

        conversionProjectApi
            .createProject(ProjectBuilder.createConversionProjectRequest())
            .then(response => {
                projectId = response.id;
            });
    });

    it("Should be able to move around the complete service", () => {
    }
}
