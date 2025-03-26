import {
    EnvApi,
    EnvClientId,
    EnvClientSecret,
    EnvCompleteApiClientId,
    EnvTenantId,
    EnvUsername
} from "../constants/cypressConstants";

it("debug auth", () => {
    const tenantId = Cypress.env(EnvTenantId);
    const clientId = Cypress.env(EnvClientId);
    const clientSecret = Cypress.env(EnvClientSecret);
    const completeApiClientId = Cypress.env(EnvCompleteApiClientId);
    const scope = `api://${completeApiClientId}/.default`;

    cy.request({
        method: "POST",
        url: `https://login.microsoftonline.com/${tenantId}/oauth2/v2.0/token`,
        form: true,
        body: {
            grant_type: "client_credentials",
            client_id: clientId,
            client_secret: clientSecret,
            scope: scope,
        },
    }).then((response) => {
        expect(response.status).to.eq(200);
        const accessToken = response.body.access_token;
        const headers = {
            Authorization: `Bearer ${accessToken}`,
            "Content-type": "application/json",
            "x-user-context-name": Cypress.env(EnvUsername),
        };

        cy.request<boolean>({
                method: "GET",
                url: Cypress.env(EnvApi) + "/v1/Projects/List/All",
                headers: headers,
            })
            .then((response) => {
                expect(response.status).to.eq(200);
            });
    });
});
