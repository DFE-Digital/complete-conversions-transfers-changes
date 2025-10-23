import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "cypress/api/apiBase";
import Chainable = Cypress.Chainable;

interface IdObject {
    value: string;
}

interface CreateLocalAuthorityRequest {
    code: string;
    name: string;
    address1: string;
    address2: string;
    address3: string;
    addressTown: string;
    addressCounty: string;
    addressPostcode: string;
    title: string;
    contactName: string;
    email: string;
    phone: string;
}

interface DeleteLocalAuthorityRequest {
    id: IdObject;
    contactId: IdObject;
}

interface LocalAuthorityItem {
    id: IdObject;
    name: string;
}

interface CreateLocalAuthorityResponse {
    localAuthorityId: IdObject;
    contactId: IdObject;
}

type ListLocalAuthoritiesResponse = LocalAuthorityItem[];

class LocalAuthorityApi extends ApiBase {
    private readonly localAuthorityUrl = `${Cypress.env(EnvApi)}/v1/ServiceSupport/LocalAuthority`;

    public createLocalAuthority(request: CreateLocalAuthorityRequest): Chainable<CreateLocalAuthorityResponse> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<CreateLocalAuthorityResponse>({
                    method: "POST",
                    url: this.localAuthorityUrl,
                    headers,
                    body: request,
                })
                .then((response) => {
                    expect(response.status, `Expected POST request to return 201 but got ${response.status}`).to.eq(
                        201,
                    );
                    return response.body;
                });
        });
    }

    public deleteLocalAuthority(id: string, contactId: string) {
        const request: DeleteLocalAuthorityRequest = {
            id: { value: id },
            contactId: { value: contactId },
        };
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request({
                    method: "DELETE",
                    url: this.localAuthorityUrl,
                    headers,
                    body: request,
                })
                .then((response) => {
                    expect(response.status, `Expected DELETE request to return 204 but got ${response.status}`).to.eq(
                        204,
                    );
                });
        });
    }

    public listLocalAuthorities(): Chainable<ListLocalAuthoritiesResponse> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<ListLocalAuthoritiesResponse>({
                    method: "GET",
                    url: `${this.localAuthorityUrl}/List/All`,
                    headers,
                    qs: { Count: 500 },
                })
                .then((response) => {
                    expect(response.status, `Expected GET request to return 200 but got ${response.status}`).to.eq(200);
                    return response.body;
                });
        });
    }

    deleteLocalAuthorityByName(name: string) {
        return this.listLocalAuthorities().then((localAuthorities) => {
            const localAuthority = localAuthorities.find((la) => la.name === name);
            if (localAuthority) {
                return this.deleteLocalAuthority(localAuthority.id.value, localAuthority.id.value);
            }
        });
    }
}

const localAuthorityApi = new LocalAuthorityApi();

export default localAuthorityApi;
