import projectRemover from "cypress/api/projectRemover";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import { beforeEach } from "mocha";
import navBar from "cypress/pages/navBar";
import groupTable from "cypress/pages/groups/groupTable";
import { dimensionsTrust, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import detailsPage from "cypress/pages/detailsPage";
import { Logger } from "cypress/common/logger";
import projectDetailsPage from "cypress/pages/projects/projectDetails/projectDetailsPage";

const groupReferenceNumber = "GRP_00000099";
const incomingTrust = macclesfieldTrust;
const outgoingTrust = dimensionsTrust;
const conversionSchoolWithGroup = ProjectBuilder.createConversionProjectRequest({
    urn: { value: 107793 },
    groupReferenceNumber: groupReferenceNumber,
    incomingTrustUkprn: { value: incomingTrust.ukprn },
});
const conversionSchoolName = "Islamia Girls' High School";
const localAuthority = "Kirklees Council";
const region = "Yorkshire and the Humber";
const transferAcademyWithGroup = ProjectBuilder.createTransferProjectRequest({
    urn: { value: 107794 },
    groupReferenceNumber: groupReferenceNumber,
    incomingTrustUkprn: { value: incomingTrust.ukprn },
    outgoingTrustUkprn: { value: outgoingTrust.ukprn },
});
const transferAcademyName = "Madni Academy";

describe("Groups tests: ", () => {
    before(() => {
        projectRemover.removeProjectIfItExists(`${conversionSchoolWithGroup.urn.value}`);
        projectRemover.removeProjectIfItExists(`${transferAcademyWithGroup.urn.value}`);
        projectApi.createConversionProject(conversionSchoolWithGroup);
        projectApi.createTransferProject(transferAcademyWithGroup);
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
            .hasValue(conversionSchoolWithGroup.urn.value)
            .summaryShows("Project type")
            .hasValue("Conversion")
            .summaryShows("Local authority")
            .hasValue(localAuthority)
            .summaryShows("Region")
            .hasValue(region)
            .hasSectionItem()
            .hasSubHeading(transferAcademyName)
            .summaryShows("URN")
            .hasValue(transferAcademyWithGroup.urn.value)
            .summaryShows("Project type")
            .hasValue("Transfer")
            .summaryShows("Local authority")
            .hasValue(localAuthority)
            .summaryShows("Region")
            .hasValue(region);
    });
});
