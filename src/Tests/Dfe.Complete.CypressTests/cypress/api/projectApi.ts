import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";
import { CreateProjectRequest, CreateProjectResponse } from "./apiDomain";

class ProjectApi extends ApiBase {
    public createProject(
        request: CreateProjectRequest,
        username? : string
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.authenticatedRequest().then((headers) => {
            if (username) {
                headers["x-user-context-name"] = username
            }
            return cy
                .request<CreateProjectResponse>({
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

const projectApi = new ProjectApi();

export default projectApi;
