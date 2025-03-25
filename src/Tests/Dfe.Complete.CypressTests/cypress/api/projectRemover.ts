import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";
import {GetProjectResponse} from "./apiDomain";
import { Logger } from "../common/logger";

class ProjectRemover extends ApiBase {
    public removeProject(urn: string): Cypress.Chainable<boolean> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<boolean>({
                    method: "DELETE",
                    url: Cypress.env(EnvApi) + "/v1/Projects",
                    headers: headers,
                    body: { value: urn },
                })
                .then((response) => {
                    return response.isOkStatusCode;
                });
        });
    }

    public getProject(urn: string): Cypress.Chainable<Cypress.Response<GetProjectResponse>> {
        return this.authenticatedRequest().then((headers) => {
            Logger.log(`token in request = ${headers.toLocaleString().substring(0, 50)}`);
            return cy
                .request<GetProjectResponse>({
                    method: "GET",
                    url: Cypress.env(EnvApi) + "/v1/Projects",
                    qs: { "urn.Value": urn },
                    headers: headers,
                })
                .then((response) => {
                    return response;
                });
        });
    }

    public removeProjectIfItExists(urn: string): Cypress.Chainable<boolean> {
        return this.getProject(urn).then((response) => {
            if (response.status === 200) {
                return this.removeProject(urn).then(() => true);
            }
            return cy.wrap(true);
        });
    }
}

const projectRemover = new ProjectRemover();

export default projectRemover;
