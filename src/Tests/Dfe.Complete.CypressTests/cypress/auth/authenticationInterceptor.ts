import { EnvAuthKey, EnvUrl, EnvUserAdId } from "../constants/cypressConstants";

export class AuthenticationInterceptor {
    register(params?: AuthenticationInterceptorParams) {
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
                    "x-user-context-role-0": "not_used", // must be present
                    "x-user-context-name": "not_used", // must be present
                    "x-user-ad-id": params?.activeDirectoryId ? params.activeDirectoryId : Cypress.env(EnvUserAdId),
                };
            },
        ).as("AuthInterceptor");
    }
}

export type AuthenticationInterceptorParams = {
    activeDirectoryId?: string;
};
