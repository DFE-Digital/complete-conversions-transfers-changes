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

class PrepareProjectApi {
    public createConversionProject(
        request: CreateConversionPrepareRequest,
    ): Cypress.Chainable<CreateConversionPrepareResponse> {
        return this.ProjectBaseRequest<CreateConversionPrepareResponse>("/projects/conversions", request, 201);
    }

    public createConversionFormAMatProject(
        request: CreateConversionFormAMatPrepareRequest,
    ): Cypress.Chainable<CreateConversionPrepareResponse> {
        return this.ProjectBaseRequest<CreateConversionPrepareResponse>(
            "/projects/conversions/form-a-mat",
            request,
            201,
        );
    }

    public createTransferProject(
        request: CreateTransferPrepareRequest,
    ): Cypress.Chainable<CreateTransferPrepareResponse> {
        return this.ProjectBaseRequest<CreateTransferPrepareResponse>("/projects/transfers", request, 201);
    }

    public createTransferFormAMatProject(
        request: CreateTransferFormAMatPrepareRequest,
    ): Cypress.Chainable<CreateTransferPrepareResponse> {
        return this.ProjectBaseRequest<CreateTransferPrepareResponse>("/projects/transfers/form-a-mat", request, 201);
    }

    private ProjectBaseRequest<T extends CreatePrepareProjectResponse>(
        path: string,
        body: PrepareProjectRequest,
        expectedStatus: number = 201,
    ): Cypress.Chainable<T> {
        return cy
            .request<T>({
                method: "POST",
                headers: {
                    apikey: Cypress.env("rubyApiKey"),
                },
                url: Cypress.env("rubyApi") + path,
                body,
            })
            .then((response) => {
                expect(
                    response.status,
                    `Expected POST request to return ${expectedStatus} but got ${response.status} with body ${response.body}`,
                ).to.eq(expectedStatus);
                return response.body;
            });
    }
}

const prepareProjectApi = new PrepareProjectApi();

export default prepareProjectApi;
