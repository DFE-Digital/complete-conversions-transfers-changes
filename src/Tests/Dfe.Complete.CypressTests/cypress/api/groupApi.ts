import { ApiBase } from "cypress/api/apiBase";
import { EnvApi } from "cypress/constants/cypressConstants";

export interface GroupModel {
    groupId: string;
    groupName: string;
    groupIdentifier: string;
    trustUkprn: string;
    includedEstablishments: string;
}

export type GroupsResponse = GroupModel[];

class GroupApi extends ApiBase {
    public getGroups(): Cypress.Chainable<GroupsResponse> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request({
                    method: "GET",
                    url: Cypress.env(EnvApi) + "/v1/ProjectGroup/List",
                    headers: headers,
                })
                .then((response) => {
                    expect(response.status).to.eq(200);
                    return response.body;
                });
        });
    }

    public getGroupBy<K extends keyof GroupModel>(property: K, value: GroupModel[K]): Cypress.Chainable<GroupModel[]> {
        return this.getGroups().then((groups) => {
            return groups.filter((group) => group[property] === value);
        });
    }
}

const groupApi = new GroupApi();

export default groupApi;
