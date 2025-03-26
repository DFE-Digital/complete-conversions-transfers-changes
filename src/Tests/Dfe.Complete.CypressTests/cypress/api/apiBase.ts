import { EnvCompleteApiClientId, EnvClientId, EnvClientSecret, EnvTenantId, EnvUsername, UserAccessToken } from "cypress/constants/cypressConstants";

export class ApiBase {
    protected getHeaders(): object {
        return {
            Authorization: 'Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkpETmFfNGk0cjdGZ2lnTDNzSElsSTN4Vi1JVSIsImtpZCI6IkpETmFfNGk0cjdGZ2lnTDNzSElsSTN4Vi1JVSJ9.eyJhdWQiOiJhcGk6Ly8zZDBmOWRjMS1kN2QyLTRjODQtOWQxYy04NzNjNzZmZWZlZmMiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85YzdkOWRkMy04NDBjLTRiM2YtODE4ZS01NTI4NjUwODJlMTYvIiwiaWF0IjoxNzQyOTgzNDIzLCJuYmYiOjE3NDI5ODM0MjMsImV4cCI6MTc0Mjk4NzMyMywiYWlvIjoiazJSZ1lEQ3Q5OXB1Nlc2bmNkcm8yOWQvWnpseUFBPT0iLCJhcHBpZCI6ImE2MjQ5MTk4LTJjMGUtNGRjNi05OGFiLWZmYThhYTliODRmMyIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzljN2Q5ZGQzLTg0MGMtNGIzZi04MThlLTU1Mjg2NTA4MmUxNi8iLCJvaWQiOiJlZGQwYzQzNS00ZmM4LTQyNzgtYjYwYi05YmJiMmRlNWU5NmMiLCJyaCI6IjEuQVRBQTA1MTluQXlFUDB1QmpsVW9aUWd1RnNHZER6M1MxNFJNblJ5SFBIYi1fdnhEQVFBd0FBLiIsInJvbGVzIjpbIkFQSS5EZWxldGUiLCJBUEkuUmVhZCIsIkFQSS5Xcml0ZSIsIkFQSS5VcGRhdGUiXSwic3ViIjoiZWRkMGM0MzUtNGZjOC00Mjc4LWI2MGItOWJiYjJkZTVlOTZjIiwidGlkIjoiOWM3ZDlkZDMtODQwYy00YjNmLTgxOGUtNTUyODY1MDgyZTE2IiwidXRpIjoibzJDN25GRFpuRU9TTFV0NjJNZE5BQSIsInZlciI6IjEuMCJ9.U36F9WLG0ZX6zmvcEj4M2keESR95qaHb5J-OBp5DgJqm4S69ATOsn8vwjLoV_daR6lYsExg-Suxtw7NRd_hwDVY2_1oyx2eFx66JbUWLoHRjl7rSllMOMDT-e7UFeV5s2XAwtNrnxWCYJeL3xjOZ6mxvFzFQqTLwJn9lgcRYpKzI_QJ8RBCLJIVgRDGZIHJABup0B-IWKVBXrt0GNLXZ2v-uxM2VscmdQkNBRtm2myXPaxR8uaSZ9QUyN2p4Z5R69P8rxC_DYTlzoCAXUeUSIzt_wYdC9iXDKp-SYL6fSg4lPke4tKBb3I3Scic1ZiZhmXskzuDb6iruk-63HyVt7Q',
            "Content-type": "application/json",
            "x-user-context-name": Cypress.env(EnvUsername),
        };
    }

    protected authenticatedRequest(): Cypress.Chainable<object> {
        // const accessToken = Cypress.env(UserAccessToken);
        // if (accessToken) {
        return cy.wrap(this.getHeaders());
        // }

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

