import { CompleteApiClientId, EnvClientId, EnvClientSecret, EnvTenantId, EnvUsername } from "cypress/constants/cypressConstants";

export class ApiBase {
    protected getHeaders(): object {
        return {
            Authorization: `Bearer ${Cypress.env('accessToken')}`,
            "Content-type": "application/json",
            "x-user-context-name": Cypress.env(EnvUsername),
        };
    }

    protected authenticatedRequest(): Cypress.Chainable<object> {
        const accessToken = Cypress.env('accessToken');
        if (accessToken) {
            return cy.wrap(this.getHeaders());
        }

        const tenantId = Cypress.env(EnvTenantId);  
        const clientId = Cypress.env(EnvClientId);
        const clientSecret = Cypress.env(EnvClientSecret);
        const completeApiClientId = Cypress.env(CompleteApiClientId);
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
            Cypress.env('accessToken', accessToken);

            return this.getHeaders();
        })
    }
}

