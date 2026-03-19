import {
    EnvClientId,
    EnvClientSecret,
    EnvCompleteApiClientId,
    EnvTenantId,
    EnvUsername,
    UserAccessToken,
} from "cypress/constants/cypressConstants";

export class ApiBase {
    protected authenticatedRequest(): Cypress.Chainable<object> {
        return cy.task<Record<string, string>>("get", [UserAccessToken]).then((result) => {
            if (result[UserAccessToken]) {
                return this.buildHeaders(result[UserAccessToken]);
            }

            return cy.env([EnvTenantId, EnvClientId, EnvClientSecret, EnvCompleteApiClientId]).then((env) => {
                const scope = `api://${env[EnvCompleteApiClientId]}/.default`;

                return cy
                    .request({
                        method: "POST",
                        url: `https://login.microsoftonline.com/${env[EnvTenantId]}/oauth2/v2.0/token`,
                        form: true,
                        body: {
                            grant_type: "client_credentials",
                            client_id: env[EnvClientId],
                            client_secret: env[EnvClientSecret],
                            scope: scope,
                        },
                    })
                    .then((response) => {
                        expect(response.status).to.eq(200);
                        const token = response.body.access_token;

                        return cy.task("set", { [UserAccessToken]: token }).then(() => {
                            return this.buildHeaders(token);
                        });
                    });
            });
        });
    }

    private buildHeaders(accessToken: string): object {
        return {
            Authorization: `Bearer ${accessToken}`,
            "Content-type": "application/json",
            "x-user-context-name": Cypress.expose(EnvUsername),
        };
    }
}
