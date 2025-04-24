import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";
import {
    CreateConversionProjectRequest,
    CreateMatConversionProjectRequest,
    CreateMatTransferProjectRequest,
    CreateProjectResponse,
    CreateTransferProjectRequest,
    ProjectRequest,
} from "./apiDomain";

class ProjectApi extends ApiBase {
    public createConversionProject(
        request: CreateConversionProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "Conversion", username);
    }

    public createTransferProject(
        request: CreateTransferProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "Transfer", username);
    }

    createMatConversionProject(
        request: CreateMatConversionProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "MatConversion", username);
    }

    createMatTransferProject(
        request: CreateMatTransferProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "MatTransfer", username);
    }

    private createProjectBase(
        request: ProjectRequest,
        projectType: string,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.authenticatedRequest().then((headers) => {
            if (username) {
                headers["x-user-context-name"] = username;
            }
            return cy
                .request<CreateProjectResponse>({
                    method: "POST",
                    url: Cypress.env(EnvApi) + "/v1/projects/Create/" + projectType,
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
