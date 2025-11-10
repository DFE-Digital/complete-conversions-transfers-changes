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
    UpdateProjectHandoverAssignRequest,
} from "./apiDomain";
import { ProjectBuilder } from "cypress/api/projectBuilder";
import { TestUser } from "cypress/constants/TestUser";

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
    public createAndUpdateConversionProject(
        request: CreateConversionProjectRequest,
        user?: TestUser,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createAndUpdateProject(
            () => this.createConversionProject(request, user?.username),
            (projectId) =>
                ProjectBuilder.updateConversionProjectHandoverAssignRequest({
                    projectId: { value: projectId },
                    ...(user && { userId: { value: user.id } }),
                }),
        );
    }
    public createConversionProject(
        request: CreateConversionProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "Conversions", username);
    }

    public createAndUpdateTransferProject(
        request: CreateTransferProjectRequest,
        user?: TestUser,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createAndUpdateProject(
            () => this.createTransferProject(request, user?.username),
            (projectId) =>
                ProjectBuilder.updateTransferProjectHandoverAssignRequest({
                    projectId: { value: projectId },
                    ...(user && { userId: { value: user.id } }),
                }),
        );
    }

    public createTransferProject(
        request: CreateTransferProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "Transfers", username);
    }

    createAndUpdateMatConversionProject(
        request: CreateMatConversionProjectRequest,
        user?: TestUser,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createAndUpdateProject(
            () => this.createMatConversionProject(request, user?.username),
            (projectId) =>
                ProjectBuilder.updateConversionProjectHandoverAssignRequest({
                    projectId: { value: projectId },
                    ...(user && { userId: { value: user.id } }),
                }),
        );
    }

    createMatConversionProject(
        request: CreateMatConversionProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "MatConversions", username);
    }

    createAndUpdateMatTransferProject(
        request: CreateMatTransferProjectRequest,
        user?: TestUser,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createAndUpdateProject(
            () => this.createMatTransferProject(request, user?.username),
            (projectId) =>
                ProjectBuilder.updateTransferProjectHandoverAssignRequest({
                    projectId: { value: projectId },
                    ...(user && { userId: { value: user.id } }),
                }),
        );
    }

    createMatTransferProject(
        request: CreateMatTransferProjectRequest,
        username?: string,
    ): Cypress.Chainable<CreateProjectResponse> {
        return this.createProjectBase(request, "MatTransfers", username);
    }

    updateProjectHandoverAssign(request: UpdateProjectHandoverAssignRequest) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<CreateProjectResponse>({
                    method: "PATCH",
                    url: Cypress.env(EnvApi) + "/v1/Projects/Project/Handover/Assign",
                    headers: headers,
                    body: request,
                })
                .then((response) => {
                    expect(response.status).to.eq(204);
                    return response.body;
                });
        });
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

    completeProject(projectId: string) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<boolean>({
                    method: "PATCH",
                    url: Cypress.env(EnvApi) + "/v1/Projects/project/Complete",
                    headers: headers,
                    body: {
                        projectId: { value: projectId },
                    },
                })
                .then((response) => {
                    return response.isOkStatusCode;
                });
        });
    }

    private createAndUpdateProject(
        createProjectFn: () => Cypress.Chainable<CreateProjectResponse>,
        buildAssignRequestFn: (projectId: string) => UpdateProjectHandoverAssignRequest,
    ): Cypress.Chainable<CreateProjectResponse> {
        return createProjectFn().then((createResponse) => {
            const projectId = createResponse.value;
            const assignRequest = buildAssignRequestFn(projectId);
            return this.updateProjectHandoverAssign(assignRequest).then(() => createResponse);
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
                    url: Cypress.env(EnvApi) + "/v1/projects/" + projectType,
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
