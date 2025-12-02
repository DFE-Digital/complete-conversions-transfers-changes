import { ApiBase } from "cypress/api/apiBase";
import { CreateContactRequest, CreateProjectResponse } from "cypress/api/apiDomain";
import { EnvApi } from "cypress/constants/cypressConstants";

class ContactApi extends ApiBase {
    public createContact(contactRequest: CreateContactRequest): Cypress.Chainable<CreateProjectResponse> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<CreateProjectResponse>({
                    method: "POST",
                    url: Cypress.env(EnvApi) + "/v1/Contacts/CreateExternalContact",
                    headers: headers,
                    body: contactRequest,
                    failOnStatusCode: false,
                })
                .then((response) => {
                    expect(response.status).to.eq(201);
                    return response.body;
                });
        });
    }
}

const contactApi = new ContactApi();

export default contactApi;
