import { EnvCompleteApiClientId, EnvClientId, EnvClientSecret, EnvTenantId, EnvUsername, UserAccessToken } from "cypress/constants/cypressConstants";
import { Logger } from "../common/logger";

export class ApiBase {
    protected getHeaders(): object {
        const token = Cypress.env(UserAccessToken);
        Logger.log(`Get headers - Token first half: ${token.substring(0, 10)}`);
        Logger.log(`Get headers - Token last half: ${token.substring(11, token.length)}`);

        return {
            Authorization: `Bearer ${token}`,
            "Content-type": "application/json",
            "x-user-context-name": Cypress.env(EnvUsername),
        };
    }

    protected authenticatedRequest(): Cypress.Chainable<object> {
        const accessToken = Cypress.env(UserAccessToken);
        if (accessToken) {
            return cy.wrap(this.getHeaders());
        }

        const tenantId = Cypress.env(EnvTenantId);  
        const clientId = Cypress.env(EnvClientId);
        const clientSecret = Cypress.env(EnvClientSecret);
        const completeApiClientId = Cypress.env(EnvCompleteApiClientId);
        const scope = `api://${completeApiClientId}/.default`;  
        
        return cy.request({
            method: 'POST',
            url: `https://login.microsoftonline.com/${tenantId}/oauth2/v2.0/token`,
            form: true,
            body: {
                grant_type: 'client_credentials',
                client_id: clientId,
                client_secret: clientSecret,
                scope: scope
            }
        }).then((response) => {
            expect(response.status).to.eq(200);
            const accessToken = response.body.access_token;
            Cypress.env(UserAccessToken, accessToken);

            return this.getHeaders();
        })
    }
}

