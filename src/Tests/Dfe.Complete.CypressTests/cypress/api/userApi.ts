import { ApiBase } from "cypress/api/apiBase";
import { EnvApi } from "cypress/constants/cypressConstants";

interface IdObject {
    value: string;
}

interface UserUpdateRequest {
    id: IdObject;
    firstName: string | null;
    lastName: string | null;
    email: string | null;
    team: string | null;
}

class UserApi extends ApiBase {
    private readonly userUrl = `${Cypress.env(EnvApi)}/v1/Users`;

    public updateUser(body: UserUpdateRequest) {
        return this.userBaseRequest<IdObject>("PUT", body);
    }

    private userBaseRequest<T>(method: string, body?: UserUpdateRequest, path = "") {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<T>({
                    method: method,
                    url: `${this.userUrl}${path}`,
                    headers,
                    body: body,
                })
                .then((response) => {
                    expect(response.isOkStatusCode).to.be.true;
                    return response.body;
                });
        });
    }
}

const userApi = new UserApi();

export default userApi;
