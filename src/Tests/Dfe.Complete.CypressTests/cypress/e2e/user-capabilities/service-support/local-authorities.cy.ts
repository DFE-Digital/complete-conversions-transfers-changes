import { serviceSupportUser } from "cypress/constants/cypressConstants";
import serviceSupport from "cypress/pages/service-support/service-support";
import localAuthoritiesPage from "cypress/pages/service-support/localAuthoritiesPage";
import newLocalAuthorityPage from "cypress/pages/service-support/newLocalAuthorityPage";
import validationComponent from "cypress/pages/validationComponent";
import localAuthorityPage from "cypress/pages/service-support/localAuthorityPage";
import editLocalAuthorityPage from "cypress/pages/service-support/editLocalAuthorityPage";
import deleteLocalAuthorityPage from "cypress/pages/service-support/deleteLocalAuthorityPage";
import { Logger } from "cypress/common/logger";
import localAuthorityApi from "cypress/api/localAuthorityApi";
import { LocalAuthorityBuilder } from "cypress/api/localAuthorityBuilder";

const localAuthority = {
    name: "Test Local Authority",
    code: "123",
    addressLines: ["Test Address Line 1", "Test Address Line 2", "Test Address Line 3"],
    town: "Test Town",
    county: "Test County",
    postcode: "W1A 1AA",
    dcsPosition: "Test Director",
    dcsName: "Test DCS Name",
    dcsEmail: "test@email.com",
    dcsPhone: "01234567890",
};
const localAuthorityToEdit = LocalAuthorityBuilder.createLocalAuthorityRequest({ name: "Test Local Auth to edit" });
const localAuthorityToDelete = LocalAuthorityBuilder.createLocalAuthorityRequest({
    name: "Test Local Auth to delete",
    code: "125",
});
const localAuthorityEdited = {
    name: "Test Local Authority - Edited",
    code: "124-edited",
    addressLines: ["Test Address Line 1 - Edited", "Test Address Line 2 - Edited", "Test Address Line 3 - Edited"],
    town: "Test Town - Edited",
    county: "Test County - Edited",
    postcode: "W1A 1AB",
    dcsPosition: "Test Director - Edited",
    dcsName: "Test DCS Name - Edited",
    dcsEmail: "edited-test@email.com",
    dcsPhone: "01234567891",
};

describe("Service support user - Local authorities: ", () => {
    before(() => {
        localAuthorityApi.createLocalAuthority(localAuthorityToEdit);
        localAuthorityApi.createLocalAuthority(localAuthorityToDelete);
    });

    beforeEach(() => {
        cy.login(serviceSupportUser);
        cy.acceptCookies();
        cy.visit("/service-support/local-authorities");
    });

    after(() => {
        localAuthorityApi.deleteLocalAuthority(localAuthorityToEdit.id.value, localAuthorityToEdit.contactId.value);
        localAuthorityApi.deleteLocalAuthority(localAuthorityToDelete.id.value, localAuthorityToDelete.contactId.value);
        localAuthorityApi.deleteLocalAuthorityByName(localAuthority.name);
    });

    it("Should be able to add a new local authority", () => {
        cy.visit("/");
        Logger.log("Navigate to Local Authorities page");
        serviceSupport.viewLocalAuthorities().containsHeading("Local authorities");

        Logger.log("Create a new local authority");
        localAuthoritiesPage.newLocalAuthority();

        Logger.log("Input local authority details and save");
        newLocalAuthorityPage
            .containsHeading("New local authority")
            .withName(localAuthority.name)
            .withCode(localAuthority.code)
            .withAddressLines(...localAuthority.addressLines)
            .withTown(localAuthority.town)
            .withCounty(localAuthority.county)
            .withPostcode(localAuthority.postcode)
            .withDCSPosition(localAuthority.dcsPosition)
            .withDCSName(localAuthority.dcsName)
            .withDCSEmail(localAuthority.dcsEmail)
            .withDCSPhone(localAuthority.dcsPhone)
            .saveAndReturn();

        Logger.log("Verify local authority was created successfully");
        validationComponent.hasNoValidationErrors();
        localAuthorityPage.authorityCreatedSuccessMessage();

        Logger.log("Verify local authority details");
        localAuthorityPage
            .containsHeading(localAuthority.name)
            .hasCode(localAuthority.code)
            .hasAddressLines(localAuthority.addressLines)
            .hasTown(localAuthority.town)
            .hasCounty(localAuthority.county)
            .hasPostcode(localAuthority.postcode)
            .hasDCSPosition(localAuthority.dcsPosition)
            .hasDCSName(localAuthority.dcsName)
            .hasDCSEmail(localAuthority.dcsEmail)
            .hasDCSPhone(localAuthority.dcsPhone);
    });

    it("Should be able to edit an existing local authority", () => {
        Logger.log("View local authority details");
        localAuthoritiesPage
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(localAuthorityToEdit.name)
            .viewLocalAuthorityDetails(localAuthorityToEdit.name);

        Logger.log("Edit local authority");
        localAuthorityPage.edit();

        Logger.log("Verify current local authority details on edit page");
        editLocalAuthorityPage
            .containsHeading(`Change details for ${localAuthorityToEdit.name}`)
            .hasCode(localAuthorityToEdit.code)
            .hasAddressLine1(localAuthorityToEdit.address1)
            .hasAddressLine2(localAuthorityToEdit.address2)
            .hasAddressLine3(localAuthorityToEdit.address3)
            .hasTown(localAuthorityToEdit.addressTown)
            .hasCounty(localAuthorityToEdit.addressCounty)
            .hasPostcode(localAuthorityToEdit.addressPostcode)
            .hasDCSPosition(localAuthorityToEdit.title)
            .hasDCSName(localAuthorityToEdit.contactName)
            .hasDCSEmail(localAuthorityToEdit.email)
            .hasDCSPhone(localAuthorityToEdit.phone);

        Logger.log("Edit local authority details and save");
        editLocalAuthorityPage
            .editCode(localAuthorityEdited.code)
            .editAddressLine1(localAuthorityEdited.addressLines[0])
            .editAddressLine2(localAuthorityEdited.addressLines[1])
            .editAddressLine3(localAuthorityEdited.addressLines[2])
            .editTown(localAuthorityEdited.town)
            .editCounty(localAuthorityEdited.county)
            .editPostcode(localAuthorityEdited.postcode)
            .editDCSPosition(localAuthorityEdited.dcsPosition)
            .editDCSName(localAuthorityEdited.dcsName)
            .editDCSEmail(localAuthorityEdited.dcsEmail)
            .editDCSPhone(localAuthorityEdited.dcsPhone)
            .saveAndReturn();

        Logger.log("Verify local authority was updated successfully with correct details");
        localAuthorityPage
            .detailsUpdatedSuccessMessage()
            .containsHeading(localAuthorityToEdit.name)
            .hasCode(localAuthorityEdited.code)
            .hasAddressLines(localAuthorityEdited.addressLines)
            .hasTown(localAuthorityEdited.town)
            .hasCounty(localAuthorityEdited.county)
            .hasPostcode(localAuthorityEdited.postcode)
            .hasDCSPosition(localAuthorityEdited.dcsPosition)
            .hasDCSName(localAuthorityEdited.dcsName)
            .hasDCSEmail(localAuthorityEdited.dcsEmail)
            .hasDCSPhone(localAuthorityEdited.dcsPhone);
    });

    it("Should show validation errors when trying to edit a local authority with blank fields or duplicate code", () => {
        Logger.log("View local authority details");
        localAuthoritiesPage
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(localAuthorityToEdit.name)
            .viewLocalAuthorityDetails(localAuthorityToEdit.name);

        Logger.log("Edit local authority");
        localAuthorityPage.edit();

        Logger.log("Submit form with invalid fields");
        editLocalAuthorityPage
            .editAddressLine1("")
            .editPostcode("invalid postcode")
            .editDCSEmail("invalid email")
            .saveAndReturn();

        Logger.log("Verify validation errors are displayed");
        validationComponent
            .hasLinkedValidationErrorForField("Address1", "Can't be blank")
            .hasLinkedValidationErrorForField("AddressPostcode", "Not recognised as a UK postcode")
            .hasLinkedValidationErrorForField("Email", "Email address must be in correct format");

        Logger.log("Revisit current URL to clear validation errors");
        cy.revisitCurrentUrl();

        Logger.log("Submit form with duplicate code");
        editLocalAuthorityPage.editCode(localAuthorityToDelete.code).saveAndReturn();

        Logger.log("Verify validation error for duplicate code");
        validationComponent.hasLinkedValidationErrorForField("Code", "Has already been taken");
    });

    it("Should show validation errors when trying to add a new local authority with no input", () => {
        Logger.log("Create new local authority");
        localAuthoritiesPage.newLocalAuthority();

        Logger.log("Submit form with no input");
        newLocalAuthorityPage.saveAndReturn();

        Logger.log("Verify validation errors are displayed");
        validationComponent
            .hasLinkedValidationErrorForField("Name", "Can't be blank")
            .hasLinkedValidationErrorForField("Code", "Can't be blank")
            .hasLinkedValidationErrorForField("Address1", "Can't be blank")
            .hasLinkedValidationErrorForField("AddressPostcode", "Can't be blank");
    });

    it("Should show 'code has already been taken' error when trying to add a new local authority with a duplicate local authority code", () => {
        Logger.log("Create new local authority");
        localAuthoritiesPage.newLocalAuthority();

        Logger.log("Input local authority details with duplicate code");
        newLocalAuthorityPage
            .withName("Test some name")
            .withCode(localAuthorityToDelete.code)
            .withAddressLines("some address")
            .withPostcode("W1A 1AA")
            .saveAndReturn();

        Logger.log("Verify validation error for duplicate code");
        validationComponent.hasLinkedValidationErrorForField("Code", "Has already been taken");
    });

    // bug 227256
    it.skip("Should be able to delete an existing local authority", () => {
        Logger.log("View local authority details");
        localAuthoritiesPage
            .goToLastPage()
            .goToPreviousPageUntilFieldIsVisible(localAuthorityToDelete.name)
            .viewLocalAuthorityDetails(localAuthorityToDelete.name);

        Logger.log("Delete local authority");

        localAuthorityPage.delete();
        Logger.log("Confirm delete local authority");

        deleteLocalAuthorityPage.hasAreYouSureYouWantToDeleteMessage(localAuthorityToDelete.name).confirmDelete();

        Logger.log("Success message is displayed and verify local authority is no longer listed");
        localAuthoritiesPage
            .authorityDeletedSuccessMessage()
            .localAuthorityDoesNotExistAcrossAllPages(localAuthorityToDelete.name);
    });
});
