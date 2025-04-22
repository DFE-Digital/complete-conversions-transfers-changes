import { UserRole } from "cypress/constants/UserRoles";

export class TestUser {
    username: string;
    adId: string;
    roles: UserRole[];

    constructor(username: string, adId: string, roles: UserRole[]) {
        this.username = username;
        this.adId = adId;
        this.roles = roles;
    }

    get email(): string {
        const [firstName, lastName] = this.username.split(" ");
        return `${firstName.toLowerCase()}.${lastName.toLowerCase()}@education.gov.uk`;
    }

    get firstName(): string {
        return this.username.split(" ")[0].toLowerCase();
    }

    get lastName(): string {
        return this.username.split(" ")[1].toLowerCase();
    }
}
