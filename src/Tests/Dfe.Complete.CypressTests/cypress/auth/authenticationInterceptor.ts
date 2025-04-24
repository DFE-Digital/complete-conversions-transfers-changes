import { cypressUser, EnvAuthKey, EnvUrl } from "../constants/cypressConstants";
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
                    "x-user-ad-id": user.adId
                };
                // set roles
                user?.roles?.forEach((role, index) => {
                    req.headers[`x-user-context-role-${index}`] = role;
                });
            },
        ).as("AuthInterceptor");
    }
}
