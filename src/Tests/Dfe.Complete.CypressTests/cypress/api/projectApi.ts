import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";
import { CreateConversionProjectRequest, CreateConversionProjectResponse } from "./apiDomain";

class ProjectApi extends ApiBase {
    public createProject(
        request: CreateConversionProjectRequest,
    ): Cypress.Chainable<CreateConversionProjectResponse> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<CreateConversionProjectResponse>({
                    method: "POST",
                    url: Cypress.env(EnvApi) + "/v1/projects/",
                    headers: headers,
                    body: request,
                })
                .then((response) => {
                    expect(response.status).to.eq(201);
                    return response.body;
                });
        });
    }
}

const conversionProjectApi = new ProjectApi();

export default conversionProjectApi;
