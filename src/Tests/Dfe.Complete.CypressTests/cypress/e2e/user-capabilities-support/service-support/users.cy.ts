import { beforeEach } from "mocha";
import { serviceSupportUser, userToEdit } from "cypress/constants/cypressConstants";
import usersPage from "cypress/pages/service-support/usersPage";
import userApi from "cypress/api/userApi";
import { Logger } from "cypress/common/logger";
import editUserDetailsPage from "cypress/pages/service-support/editUserDetailsPage";

const userNewDetails = {
    firstName: "EditedFirstName",
    lastName: "EditedLastName",
    email: "editedEmail@education.gov.uk",
    team: "East of England",
};

describe("Service support user - Local authorities: ", () => {
    before(() => {
        userApi.updateUser({
            id: {
                value: userToEdit.id,
            },
            firstName: userToEdit.firstName,
            lastName: userToEdit.lastName,
            email: userToEdit.email,
            team: "London",
        });
    });

    beforeEach(() => {
        cy.login(serviceSupportUser);
        cy.acceptCookies();
        cy.visit("/service-support/users");
    });

    it("Should be able to view all users and view a user's details", () => {
        Logger.log("Verify users table has correct data");
        usersPage
            .containsHeading("Users")
            .hasTableHeaders(["Name", "Email", "Team", "Last seen", "Edit user"])
            .goToNextPageUntilFieldIsVisible(userToEdit.username)
            .withUser(userToEdit.username)
            .columnHasValue("Email", userToEdit.email)
            .columnHasValue("Team", "London")
            .columnHasValue("Last seen", "N/A");
    });

    it("Should be able to edit a user's details", () => {
        Logger.log("Find and edit user");
        usersPage.goToNextPageUntilFieldIsVisible(userToEdit.username).editUser(userToEdit.username);

        Logger.log("Update user details");
        editUserDetailsPage
            .withFirstName(userNewDetails.firstName)
            .withLastName(userNewDetails.lastName)
            .withEmail(userNewDetails.email)
            .withTeam(userNewDetails.team)
            .saveAndReturn();

        Logger.log("Verify user details have been updated");
        usersPage.containsSuccessBannerWithMessage(`User ${userNewDetails.email} updated successfully`);
        usersPage
            .goToNextPageUntilFieldIsVisible(`${userNewDetails.firstName} ${userNewDetails.lastName}`)
            .editUser(`${userNewDetails.firstName} ${userNewDetails.lastName}`);
        editUserDetailsPage
            .hasFirstName(userNewDetails.firstName)
            .hasLastName(userNewDetails.lastName)
            .hasEmail(userNewDetails.email)
            .hasTeam(userNewDetails.team);
    });

    it("Should have access to create a user", () => {
        Logger.log("Verify create user button");
        usersPage.clickButton("Add a new user").hasButton("Save and return");
    });
});
