import { EnvCompleteApiClientId, EnvClientId, EnvClientSecret, EnvTenantId, EnvUsername, UserAccessToken } from "cypress/constants/cypressConstants";

export class ApiBase {
    protected getHeaders(): object {
        return {
            Authorization: `Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkpETmFfNGk0cjdGZ2lnTDNzSElsSTN4Vi1JVSIsImtpZCI6IkpETmFfNGk0cjdGZ2lnTDNzSElsSTN4Vi1JVSJ9.eyJhdWQiOiJhcGk6Ly8zZDBmOWRjMS1kN2QyLTRjODQtOWQxYy04NzNjNzZmZWZlZmMiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85YzdkOWRkMy04NDBjLTRiM2YtODE4ZS01NTI4NjUwODJlMTYvIiwiaWF0IjoxNzQyOTc5MzkyLCJuYmYiOjE3NDI5NzkzOTIsImV4cCI6MTc0Mjk4MzI5MiwiYWlvIjoiazJSZ1lCRFJuUFZtOVhScFV4L2ZGWUUvNTRuZEJRQT0iLCJhcHBpZCI6ImE2MjQ5MTk4LTJjMGUtNGRjNi05OGFiLWZmYThhYTliODRmMyIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzljN2Q5ZGQzLTg0MGMtNGIzZi04MThlLTU1Mjg2NTA4MmUxNi8iLCJvaWQiOiJlZGQwYzQzNS00ZmM4LTQyNzgtYjYwYi05YmJiMmRlNWU5NmMiLCJyaCI6IjEuQVRBQTA1MTluQXlFUDB1QmpsVW9aUWd1RnNHZER6M1MxNFJNblJ5SFBIYi1fdnhEQVFBd0FBLiIsInJvbGVzIjpbIkFQSS5EZWxldGUiLCJBUEkuUmVhZCIsIkFQSS5Xcml0ZSIsIkFQSS5VcGRhdGUiXSwic3ViIjoiZWRkMGM0MzUtNGZjOC00Mjc4LWI2MGItOWJiYjJkZTVlOTZjIiwidGlkIjoiOWM3ZDlkZDMtODQwYy00YjNmLTgxOGUtNTUyODY1MDgyZTE2IiwidXRpIjoidjZia25jeFV0a2lrZXlRUzE2MXBBQSIsInZlciI6IjEuMCJ9.T4uoP8XrdrvAp-zFNgIfgJco5oQvCwWDzfRYkWsynkQp-OcPw3N5LGcCgyh9tKccVeKdm_V-yGAsxhRyIWpcrwfr8nDJcFV-F6D_CDgs-_5-THNAV-Y8YjFilcQLaqRZyCUNzsOBpdn6VKlk2locdXq6fuyh4cn635QgaIUevI33M2dP9c-Dy-VFGu-Hqf8CIJ3XGVUVHApI0OfTpFWswZl7eESyEG70rDOhWl1NaYWiET08rZ4CwnnS6vJ-Zg8thx5W4KAZMUrEmeWkJS1T-GFfy8Oy-ZiPPSIGOaX36IXDbhZFqyrzIvQQB1x6NHNu84unjTUmU2eSGUPqUneUhQ`,
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

