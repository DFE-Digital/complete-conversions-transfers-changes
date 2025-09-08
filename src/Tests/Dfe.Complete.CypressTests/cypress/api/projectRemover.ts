import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";
import projectApi from "cypress/api/projectApi";

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

        return projectApi.getProject(Number(urn)).then((response) => {
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
