import { UserRole } from "cypress/constants/UserRoles";

export class TestUser {
    id: string;
    username: string;
    adId: string;
    role: UserRole;

    constructor(id: string, username: string, adId: string, role: UserRole) {
        this.id = id;
        this.username = username;
        this.adId = adId;
        this.role = role;
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
