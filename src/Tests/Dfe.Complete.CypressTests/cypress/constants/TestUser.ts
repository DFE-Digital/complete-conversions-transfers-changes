export class TestUser {
    id: string;
    username: string;
    adId: string;

    constructor(id: string, username: string, adId: string) {
        this.id = id;
        this.username = username;
        this.adId = adId;
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
