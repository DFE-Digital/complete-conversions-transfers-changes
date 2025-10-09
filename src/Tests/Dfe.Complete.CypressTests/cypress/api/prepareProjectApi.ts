import {
    CreateConversionFormAMatPrepareRequest,
    CreateConversionPrepareRequest,
    CreateConversionPrepareResponse,
    CreatePrepareProjectResponse,
    CreateTransferFormAMatPrepareRequest,
    CreateTransferPrepareRequest,
    CreateTransferPrepareResponse,
    PrepareProjectRequest,
} from "cypress/api/apiDomain";
import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "cypress/api/apiBase";

class PrepareProjectApi extends ApiBase {
    public createConversionProject(
        request: CreateConversionPrepareRequest,
    ): Cypress.Chainable<CreateConversionPrepareResponse> {
        return this.ProjectBaseRequest<CreateConversionPrepareResponse>("/conversions", request, 201);
    }

    public createConversionFormAMatProject(
        request: CreateConversionFormAMatPrepareRequest,
    ): Cypress.Chainable<CreateConversionPrepareResponse> {
        return this.ProjectBaseRequest<CreateConversionPrepareResponse>("/conversions/form-a-mat", request, 201);
    }

    public createTransferProject(
        request: CreateTransferPrepareRequest,
    ): Cypress.Chainable<CreateTransferPrepareResponse> {
        return this.ProjectBaseRequest<CreateTransferPrepareResponse>("/transfers", request, 201);
    }

    public createTransferFormAMatProject(
        request: CreateTransferFormAMatPrepareRequest,
    ): Cypress.Chainable<CreateTransferPrepareResponse> {
        return this.ProjectBaseRequest<CreateTransferPrepareResponse>("/transfers/form-a-mat", request, 201);
    }

    private ProjectBaseRequest<T extends CreatePrepareProjectResponse>(
        path: string,
        body: PrepareProjectRequest,
        expectedStatus: number = 201,
    ): Cypress.Chainable<T> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<T>({
                    method: "POST",
                    url: Cypress.env(EnvApi) + "/v1/Projects/projects" + path,
                    headers: headers,
                    body,
                })
                .then((response) => {
                    expect(
                        response.status,
                        `Expected POST request to return ${expectedStatus} but got ${response.status} with body ${response.body}`,
                    ).to.eq(expectedStatus);
                    return response.body;
                });
        });
    }
}

const prepareProjectApi = new PrepareProjectApi();

export default prepareProjectApi;
