import projectRemover from "cypress/api/projectRemover";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { beforeEach } from "mocha";
import navBar from "cypress/pages/navBar";
import groupTable from "cypress/pages/groups/groupTable";
import { dimensionsTrust } from "cypress/constants/stringTestConstants";
import detailsPage from "cypress/pages/detailsPage";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";
import { urnPool } from "cypress/constants/testUrns";

const groupReferenceNumber = "GRP_00000099";
const incomingTrust = {
    name: "Chancery Education Trust",
    ukprn: 10058901,
    referenceNumber: "TR01647",
};
const outgoingTrustUkprn = dimensionsTrust.ukprn;
const conversionSchoolWithGroup = ProjectBuilder.createConversionProjectRequest({
    urn: urnPool.listings.islamia,
    groupId: groupReferenceNumber,
    incomingTrustUkprn: incomingTrust.ukprn,
});
const conversionSchoolName = "Islamia Girls' High School";
const localAuthority = "Kirklees Council";
const region = "Yorkshire and the Humber";
const transferAcademyWithGroup = ProjectBuilder.createTransferProjectRequest({
    urn: urnPool.listings.madni,
    groupId: groupReferenceNumber,
    incomingTrustUkprn: incomingTrust.ukprn,
    outgoingTrustUkprn: outgoingTrustUkprn,
});
const transferAcademyName = "Madni Academy";

describe("Groups tests: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(conversionSchoolWithGroup.urn);
        projectRemover.removeProjectIfItExists(transferAcademyWithGroup.urn);
        projectApi.createAndUpdateConversionProject(conversionSchoolWithGroup);
        projectApi.createAndUpdateTransferProject(transferAcademyWithGroup);
    });

    beforeEach(() => {
        cy.login();
        cy.acceptCookies();
        cy.visit(`/`);
    });

    it("Should be able to view group details and the schools and academies in the group", () => {
        Logger.log("Go to groups");
        navBar
            .goToGroups()
            .containsHeading("Groups")
            .contains("Schools and academies joining an existing trust together.")
            .contains("Groups can include any type of conversion or transfer.");

        Logger.log("Verify groups table data");
        groupTable
            .goToNextPageUntilFieldIsVisible(incomingTrust.name.toUpperCase())
            .hasTableHeaders(["Group", "Group reference number", "Trust UKPRN", "Schools or academies included"])
            .withGroup(incomingTrust.name.toUpperCase()) // bug 208086
            .columnHasValue("Group reference number", groupReferenceNumber)
            .columnHasValue("Trust UKPRN", `${incomingTrust.ukprn}`)
            .columnContainsValue("Schools or academies included", conversionSchoolName)
            .goTo(incomingTrust.name.toUpperCase()); // bug 208086

        Logger.log("Verify group's school and academy details");
        projectDetailsPage
            .containsHeading(incomingTrust.name.toUpperCase()) // bug 208086
            .contains(`Trust reference number: ${incomingTrust.referenceNumber}`)
            .contains(`Group reference number: ${groupReferenceNumber}`)
            .contains("Number in group: 2")
            .contains("Schools or academies in this group");

        detailsPage
            .inOrder()
            .hasSectionItem()
            .hasSubHeading(conversionSchoolName)
            .summaryShows("URN")
            .hasValue(conversionSchoolWithGroup.urn)
            .summaryShows("Project type")
            .hasValue("Conversion")
            .summaryShows("Local authority")
            .hasValue(localAuthority)
            .summaryShows("Region")
            .hasValue(region)
            .hasSectionItem()
            .hasSubHeading(transferAcademyName)
            .summaryShows("URN")
            .hasValue(transferAcademyWithGroup.urn)
            .summaryShows("Project type")
            .hasValue("Transfer")
            .summaryShows("Local authority")
            .hasValue(localAuthority)
            .summaryShows("Region")
            .hasValue(region);
    });
});
