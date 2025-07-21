import { serviceSupportUser } from "cypress/constants/cypressConstants";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { nextMonth, nextMonthShort } from "cypress/constants/stringTestConstants";
import serviceSupport from "cypress/pages/service-support/service-support";
import conversionURNsPage from "cypress/pages/service-support/conversionURNsPage";
import { Logger } from "cypress/common/logger";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import conversionURNsTable from "cypress/pages/projects/tables/conversionURNsTable";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import validationComponent from "cypress/pages/validationComponent";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";

const project = ProjectBuilder.createConversionProjectRequest(nextMonth);
const schoolName = "St Chad's Catholic Primary School";
const academy = {
    urn: 103846,
    name: "Cradley CofE Primary School",
    address: "Church Road",
    localAuthority: "Dudley",
    schoolPhase: "Primary",
};
const project2 = ProjectBuilder.createConversionProjectRequest(nextMonth, 103845);
const schoolName2 = "Jesson's CofE Primary School (VA)";
const projectWithAcademy = ProjectBuilder.createConversionProjectRequest(nextMonth);
const schoolWithAcademyName = "St Chad's Catholic Primary School";

describe("Service support user - Conversion URNs: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${project.urn.value}`);
        projectRemover.removeProjectIfItExists(`${project2.urn.value}`);
        projectRemover.removeProjectIfItExists(`${projectWithAcademy.urn.value}`);
        projectApi.createConversionProject(project);
        projectApi.createConversionProject(project2);
        projectApi.createConversionProject(projectWithAcademy).then((response) => {
            projectApi.updateProjectAcademyUrn(response.value, academy.urn);
        });
    });
    beforeEach(() => {
        cy.login(serviceSupportUser);
        cy.acceptCookies();
        cy.visit("/projects/service-support/without-academy-urn");
    });

    it("Should be able to view unopened academies on the URNs to create page", () => {
        Logger.log("Navigating to the URNs to create page");
        serviceSupport.viewConversionURNs().containsHeading("Conversion URNs");
        conversionURNsPage.viewURNsToCreate().containsSubHeading("URNs to create");

        Logger.log("Verify that unopened academy project is displayed with correct details");
        conversionURNsPage.goToLastPage().goToPreviousPageUntilFieldIsVisible(schoolName);
        projectTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Conversion date",
                "Academy name",
                "Academy URN",
                "View Project",
            ])
            .withSchool(schoolName)
            .columnHasValue("URN", `${project.urn.value}`)
            .columnHasValue("Conversion date", nextMonthShort)
            .columnHasValue("Academy name", "Unconfirmed")
            .columnHasValue("Academy URN", "Create academy URN")
            .columnHasValue("View Project", "View project");
    });

    it("Should be able create an academy URN for an unopened academy", () => {
        Logger.log("Navigating to the project on the URNs to create page");
        conversionURNsPage.goToLastPage().goToPreviousPageUntilFieldIsVisible(schoolName);

        Logger.log("Clicking on the Create academy URN button");
        conversionURNsTable.createAcademyUrn(schoolName);

        Logger.log("Entering the academy URN");
        conversionURNsPage
            .containsHeading(`Create academy URN for ${schoolName} conversion`)
            .contains("This is the URN for the academy that the school is converting to.")
            .contains("To add the academy URN you must have already created a new academy in GIAS.")
            .contains(
                "It can take up to 24 hours for changes to the academy URN to appear on the project information page.",
            )
            .hasLabel("Enter academy URN")
            .contains("A URN is a 6 digit number")
            .enterAcademyURN(academy.urn)
            .clickButton("Save and return");

        Logger.log("Confirming the details of the new URN");
        projectDetailsPage
            .containsHeading("Are these details correct?")
            .contains(`GIAS (Get Information about Schools) has the following information for URN ${academy.urn}:`)
            .inOrder()
            .summaryShows("Academy name")
            .hasValue(academy.name)
            .summaryShows("Academy address")
            .containsValue(academy.address)
            .summaryShows("Local authority")
            .hasValue(academy.localAuthority)
            .summaryShows("School phase")
            .hasValue(academy.schoolPhase)
            .clickButton("Yes, save and return");

        Logger.log("Success message should be displayed on Conversion URNs page");
        conversionURNsPage
            .containsHeading("Conversion URNs")
            .containsSuccessBannerWithMessage(
                `Academy URN ${academy.urn} added to ${schoolName}, ${project.urn.value}`,
            );
    });

    it("Should be able to view academies with the URNs added", () => {
        Logger.log("Navigating to the URNs added page");
        conversionURNsPage.viewURNsAdded().containsSubHeading("URNs added");

        Logger.log("Verify that the academy project is displayed with correct details");
        conversionURNsPage.goToLastPage().goToPreviousPageUntilFieldIsVisible(schoolWithAcademyName);
        conversionURNsTable
            .hasTableHeaders([
                "School or academy",
                "URN",
                "Conversion date",
                "Academy name",
                "Academy URN",
                "View Project",
            ])
            .withSchool(schoolWithAcademyName)
            .columnHasValue("URN", `${projectWithAcademy.urn.value}`)
            .columnHasValue("Conversion date", nextMonthShort)
            .columnHasValue("Academy name", academy.name)
            .columnHasValue("Academy URN", `${academy.urn}`)
            .columnHasValue("View Project", "View project")
            .viewProject(schoolWithAcademyName);

        Logger.log("Verify that the project details page is displayed");
        projectDetailsPage.containsHeading(schoolName);
    });

    it("Should be shown error when an invalid academy URN is entered and be able to cancel", () => {
        Logger.log("Navigating to the project on the URNs to create page");
        conversionURNsPage.goToLastPage().goToPreviousPageUntilFieldIsVisible(schoolName2);

        Logger.log("Clicking on the Create academy URN button");
        conversionURNsTable.createAcademyUrn(schoolName2);

        Logger.log("Entering invalid academy URN");
        conversionURNsPage.enterAcademyURN(1234567).clickButton("Save and return");

        Logger.log("Verifying that the error message is displayed");
        validationComponent.hasLinkedValidationError(
            "Please enter a valid URN. The URN must be 6 digits long. For example, 123456.",
        );

        Logger.log("Clicking on the Cancel button");
        conversionURNsPage.clickLink("Cancel");

        Logger.log("Verifying that the URNs to create page is displayed");
        conversionURNsPage.containsHeading("Conversion URNs").contains("URNs to create");
    });

    it("Should be shown 'no information available' when no academy URN is unavailable", () => {
        Logger.log("Navigating to the project on the URNs to create page");
        conversionURNsPage.goToLastPage().goToPreviousPageUntilFieldIsVisible(schoolName2);

        Logger.log("Clicking on the Create academy URN button");
        conversionURNsTable.createAcademyUrn(schoolName2);

        Logger.log("Entering URN for an unaailable academy");
        conversionURNsPage.enterAcademyURN(234567).clickButton("Save and return");

        Logger.log("No information message should be displayed");
        conversionURNsPage
            .containsHeading(`No information found for URN 234567`)
            .contains(
                "GIAS (Get Information about Schools) may not be up-to-date. It can take up to 24 hours for changes to appear in GIAS.",
            )
            .contains("Try again in 24 hours if the URN you entered is correct.")
            .contains("Check the URN you entered is correct. Enter it again if it was incorrect.");

        Logger.log("Clicking 'Enter URN again'");
        conversionURNsPage.clickButton("Enter URN again");

        Logger.log("Verify that the enter academy URN page is displayed");
        conversionURNsPage.containsHeading(`Create academy URN for ${schoolName2} conversion`);
    });

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
