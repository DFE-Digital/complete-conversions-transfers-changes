export class TestUser {
    username: string;
    adId: string;
    team: string;
    manageTeam: number;
    assignToProject: number;
    manageUserAccounts: number;
    manageConversionURNs: number;
    manageLocalAuthorities: number;

    constructor(
        username: string,
        adId: string,
        team: string,
        manageTeam: number = 0,
        assignToProject: number = 1,
        manageUserAccounts: number = 0,
        manageConversionURNs: number = 0,
        manageLocalAuthorities: number = 0,
    ) {
        this.username = username;
        this.adId = adId;
        this.team = team;
        this.manageTeam = manageTeam;
        this.assignToProject = assignToProject;
        this.manageUserAccounts = manageUserAccounts;
        this.manageConversionURNs = manageConversionURNs;
        this.manageLocalAuthorities = manageLocalAuthorities;
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
