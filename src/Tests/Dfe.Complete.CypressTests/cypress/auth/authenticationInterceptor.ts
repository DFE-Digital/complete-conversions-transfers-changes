import { cypressUser, EnvAuthKey, EnvUrl, userType } from "../constants/cypressConstants";
import { TestUser } from "cypress/constants/TestUser";

export class AuthenticationInterceptor {
    register(user: TestUser = cypressUser) {
        cy.intercept(
            {
                url: Cypress.env(EnvUrl) + "/**",
                middleware: true,
            },
            (req) => {
                // Set an auth header on every request made by the browser
                req.headers = {
                    ...req.headers,
                    Authorization: `Bearer ${Cypress.env(EnvAuthKey)}`,
                    "x-user-context-name": user.username, // must be present, but not used
                    "x-user-context-id": user.id, // must be present for antiforgery claims
                    "x-user-ad-id": user.adId,
                    "x-cypress-user": userType,
                };
            },
        ).as("AuthInterceptor");
    }
}
