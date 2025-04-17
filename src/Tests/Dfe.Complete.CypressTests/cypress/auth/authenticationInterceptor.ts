import { cypressUser, EnvAuthKey, EnvUrl } from "../constants/cypressConstants";

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
                    "x-user-ad-id": params?.activeDirectoryId ? params.activeDirectoryId : cypressUser.adId,
                };
            },
        ).as("AuthInterceptor");
    }
}

export type AuthenticationInterceptorParams = {
    activeDirectoryId?: string;
};
