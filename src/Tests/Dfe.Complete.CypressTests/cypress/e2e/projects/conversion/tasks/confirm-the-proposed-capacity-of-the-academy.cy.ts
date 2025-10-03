import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import taskListPage from "cypress/pages/projects/tasks/taskListPage";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import taskPage from "cypress/pages/projects/tasks/taskPage";
import { Logger } from "cypress/common/logger";
import validationComponent from "cypress/pages/validationComponent";
import { urnPool } from "cypress/constants/testUrns";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: { value: urnPool.conversion.stChads },
});
let projectId: string;
const otherUserProject = ProjectBuilder.createConversionFormAMatProjectRequest({
    urn: { value: urnPool.conversion.whitchurch },
    userAdId: rdoLondonUser.adId,
});
let otherUserProjectId: string;

describe("Conversion tasks - Confirm the proposed capacity of the academy", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn.value);
        projectRemover.removeProjectIfItExists(otherUserProject.urn.value);
        projectApi.createConversionProject(project).then((createResponse) => (projectId = createResponse.value));
        projectApi.createMatConversionProject(otherUserProject).then((createResponse) => {
            otherUserProjectId = createResponse.value;
        });
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${projectId}/tasks/proposed_capacity_of_the_academy`);
    });

    it("should expand and collapse guidance details", () => {
        taskPage
            .clickDropdown("Why pupil capacity is needed")
            .hasDropdownContent("Pupil capacity numbers enable the ESFA")
            .clickDropdown("Help confirming the academy's proposed capacity")
            .hasDropdownContent(
                "The proposed capacity is the maximum number of pupils and students the academy can teach.",
            );
    });

    it("should submit the form and persist selections", () => {
        Logger.log("Input all fields and save");
        taskPage
            .hasCheckboxLabel("What is the proposed capacity for pupils in reception to year 6?")
            .input("0")
            .hasCheckboxLabel("What is the proposed capacity for pupils in years 7 to 11?")
            .input("150")
            .hasCheckboxLabel("What is the proposed capacity for students in year 12 or above?")
            .input("100")
            .saveAndReturn();
        validationComponent.hasNoValidationErrors();
        taskListPage
            .hasTaskStatusCompleted("Confirm the proposed capacity of the academy")
            .selectTask("Confirm the proposed capacity of the academy");

        Logger.log("Confirm the input has persisted");
        taskPage
            .hasCheckboxLabel("What is the proposed capacity for pupils in reception to year 6?")
            .hasValue("0")
            .hasCheckboxLabel("What is the proposed capacity for pupils in years 7 to 11?")
            .hasValue("150")
            .hasCheckboxLabel("What is the proposed capacity for students in year 12 or above?")
            .hasValue("100")
            .saveAndReturn();
        validationComponent.hasNoValidationErrors();
    });

    it("should show validation errors when invalid data is entered", () => {
        taskPage
            .hasCheckboxLabel("What is the proposed capacity for pupils in reception to year 6?")
            .input("Some text")
            .hasCheckboxLabel("What is the proposed capacity for pupils in years 7 to 11?")
            .input("")
            .hasCheckboxLabel("What is the proposed capacity for students in year 12 or above?")
            .input("-1")
            .saveAndReturn();
        validationComponent
            .hasLinkedValidationErrorForField("reception-to-six-years", "Proposed capacity must be a number, like 345")
            .hasLinkedValidationErrorForField(
                "seven-to-eleven-years",
                "Enter the proposed capacity for pupils in years 7 to 11",
            )
            .hasLinkedValidationErrorForField("twelve-or-above-years", "Proposed capacity must be a number, like 345");
    });

    it("should submit the form when selecting not applicable", () => {
        taskPage.tickNotApplicable().saveAndReturn();
        validationComponent.hasNoValidationErrors();
        taskListPage
            .hasTaskStatusNotApplicable("Confirm the proposed capacity of the academy")
            .selectTask("Confirm the proposed capacity of the academy");
        taskPage.hasCheckboxLabel("Not applicable").isTicked();
    });

    it("Should NOT see the 'save and return' button for another user's project", () => {
        cy.visit(`projects/${otherUserProjectId}/tasks/proposed_capacity_of_the_academy`);
        taskPage.noSaveAndReturnExists();
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
