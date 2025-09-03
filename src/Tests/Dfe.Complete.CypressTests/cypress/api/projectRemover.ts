import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";
import { GetProjectResponse } from "./apiDomain";

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

    public removeProjectIfItExists(
        urn: string,
        maxAttempts: number = 10,
        currentAttempt: number = 0,
    ): Cypress.Chainable<boolean> {
        // fix issue where there are multiple projects with the same URN

        if (currentAttempt >= maxAttempts) {
            cy.log(`Max attempts (${maxAttempts}) reached when trying to remove project ${urn}`);
            return cy.wrap(false);
        }

        return this.getProject(urn).then((response) => {
            if (response.status === 200) {
                return this.removeProject(urn).then(() => {
                    // Recursively call this method to check if more projects exist
                    return this.removeProjectIfItExists(urn, maxAttempts, currentAttempt + 1);
                });
            }
            return cy.wrap(true);
        });
    }
}

const projectRemover = new ProjectRemover();

export default projectRemover;
