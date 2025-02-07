import { EnvApi } from "cypress/constants/cypressConstants";
import { ApiBase } from "./apiBase";

class ProjectRemover extends ApiBase {
    public removeProject(urn: string): Cypress.Chainable<boolean> {
        return cy.request<boolean>({
            method: 'DELETE',
            url: Cypress.env(EnvApi) + "/v1/Projects",
            headers: this.getHeaders(),
            body: { value : urn }
        })
        .then(response => {
            return response.isOkStatusCode;
        });
    }
}

const projectRemover = new ProjectRemover();

export default projectRemover;
