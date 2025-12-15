import { ProjectBuilder } from "cypress/api/projectBuilder";
import { nextMonthShort } from "cypress/constants/stringTestConstants";
import { before } from "mocha";
import search from "cypress/pages/search";
import searchResultsPage from "cypress/pages/searchResultsPage";
import { projectTable } from "cypress/pages/projects/tables/projectTable";
import { cypressUser } from "cypress/constants/cypressConstants";
import projectRemover from "cypress/api/projectRemover";
import projectApi from "cypress/api/projectApi";
import { checkAccessibilityAcrossPages } from "cypress/support/reusableTests";
import { urnPool } from "cypress/constants/testUrns";
import { getSignificantDateString } from "cypress/support/formatDate";

const project = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.listings.heles,
    provisionalConversionDate: getSignificantDateString(1),
});
const schoolName = "Hele's School";

describe("Search bar tests", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(project.urn);
        projectApi.createAndUpdateConversionProject(project);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit("/");
    });

    const searchCases = [
        {
            description: "should be able to search for a project by URN",
            searchTerm: `${project.urn}`,
        },
        {
            description: "Should be able to search for a project by school name",
            searchTerm: schoolName,
        },
        {
            description: "Should be able to search for a project by UKPRN",
            searchTerm: `${project.incomingTrustUkprn}`,
        },
        {
            description: "Should be able to search for a project by establishment name",
            searchTerm: "4179",
        },
        {
            description: "Should be able to search for a project by partial school name",
            searchTerm: "Hele",
        },
    ];

    for (const { description, searchTerm } of searchCases) {
        it(description, () => {
            search.clickSearch().searchFor(searchTerm);
            searchResultsPage.hasSearchResultsTitle(searchTerm).goToNextPageUntilFieldIsVisible(schoolName);
            projectTable
                .hasTableHeaders([
                    "School or academy",
                    "URN",
                    "Project type",
                    "Conversion or transfer date",
                    "Assigned to",
                ])
                .withSchool(schoolName)
                .columnHasValue("URN", `${project.urn}`)
                .columnHasValue("Project type", "Conversion")
                .columnHasValue("Conversion or transfer date", nextMonthShort)
                .columnHasValue("Assigned to", cypressUser.username)
                .goTo(schoolName);
        });
    }

    it("Check accessibility across pages", () => {
        checkAccessibilityAcrossPages();
    });
});
