import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";

class ProjectRemover extends ApiBase {
    public removeProject(urn: string): Cypress.Chainable<boolean> {
        return this.authenticatedRequest()
            .then((headers) => {
                return cy.request<boolean>({
                    method: 'DELETE',
                    url: Cypress.env(EnvApi) + "/v1/Projects",
                    headers: headers,
                    body: { value: urn }
                })
                    .then(response => {
                        return response.isOkStatusCode;
                    });
            });
    }

    public getProject(urn: string): Cypress.Chainable<number> {
        return this.authenticatedRequest()
            .then((headers) => {
                return cy.request<boolean>({
                    method: 'GET',
                    url: Cypress.env(EnvApi) + "/v1/Projects",
                    qs: { "urn.Value" : urn },
                    headers: headers,
                    failOnStatusCode: false
                })
                    .then(response => {
                        return response.status;
                    });
            });
    }

    public removeProjectIfItExists(urn: string): Cypress.Chainable<boolean> {
        return this.getProject(urn)
            .then((status) => {
                if (status === 200) {
                    return this.removeProject(urn).then(() => true);
                }
                return cy.wrap(true);
            });
    }
}

const projectRemover = new ProjectRemover();

export default projectRemover;