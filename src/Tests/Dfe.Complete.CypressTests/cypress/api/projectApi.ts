import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";
import {
    CreateConversionProjectRequest,
    CreateMatConversionProjectRequest,
    CreateMatTransferProjectRequest,
    CreateProjectResponse,
    CreateTransferProjectRequest,
    GetProjectResponse,
    ProjectRequest,
} from "./apiDomain";

class ProjectApi extends ApiBase {
    public getProject(urn: number): Cypress.Chainable<Cypress.Response<GetProjectResponse>> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<GetProjectResponse>({
                    method: "GET",
                    url: Cypress.env(EnvApi) + "/v1/Projects",
                    qs: { "urn.Value": urn },
                    headers: headers,
                    failOnStatusCode: false,
                })
                .then((response) => {
                    return response;
                });
        });
    }
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

    updateProjectAcademyUrn(projectId: string, academyUrn: number) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<CreateProjectResponse>({
                    method: "PATCH",
                    url: Cypress.env(EnvApi) + "/v1/Projects/Project/AcademyUrn",
                    headers: headers,
                    body: {
                        projectId: { value: projectId },
                        urn: { value: academyUrn },
                    },
                })
                .then((response) => {
                    expect(response.status).to.eq(204);
                    return response.body;
                });
        });
    }

    updateProjectSignificantDate(projectId: string, significantDate: string, reasonNotes: any, userId: string) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<CreateProjectResponse>({
                    method: "PATCH",
                    url: Cypress.env(EnvApi) + "/v1/Projects/Project/SignificantDate",
                    headers: headers,
                    body: {
                        projectId: { value: projectId },
                        significantDate: significantDate,
                        reasonNotes: reasonNotes,
                        userId: { value: userId },
                    },
                })
                .then((response) => {
                    expect(response.status).to.eq(204);
                    return response.body;
                });
        });
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
