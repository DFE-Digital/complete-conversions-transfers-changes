import { TestUser } from "cypress/constants/TestUser";

export const EnvUrl = "url";
export const EnvApi = "api";
export const EnvUsername = "username";
export const EnvAuthKey = "authKey";
export const EnvTenantId = "tenantId";
export const EnvClientId = "clientId";
export const EnvClientSecret = "clientSecret";
export const EnvCompleteApiClientId = "completeApiClientId";
export const UserAccessToken = "accessToken";
export const userType = "cypressUser";

// test users
export const cypressUser = new TestUser("C29AF147-F2F5-4D30-B8A5-C68BF83A148A", "cypress testuser", "TEST-AD-ID");
export const rdoLondonUser = new TestUser(
    "FD190446-DAFB-4ED7-8FB5-1AB473DDD114",
    "cypress rdo-london",
    "TEST-AD-ID-RDO",
);
export const rdoTeamLeaderUser = new TestUser(
    "B0DFB912-F806-4FC2-837F-FBE2F1779789",
    "cypress rdo-team-leader",
    "TEST-AD-ID-RDO-TL",
);
export const regionalCaseworkerUser = new TestUser(
    "C8371E6E-FD7F-42F7-8E38-F58E7308962E",
    "cypress regional-casework-services",
    "TEST-AD-ID-RCS",
);
export const regionalCaseworkerTeamLeaderUser = new TestUser(
    "A7FC973C-893A-472B-89F0-5CEE72C70C80",
    "cypress rcs-team-leader",
    "TEST-AD-ID-RCS-TL",
);
export const businessSupportUser = new TestUser(
    "B62A3AC5-2C54-4E85-8A83-DE9199D0BCCE",
    "cypress business-support",
    "TEST-AD-ID-BS",
);
export const dataConsumerUser = new TestUser(
    "9353003F-3359-4684-A696-5A96CDAB43E2",
    "cypress data-consumers",
    "TEST-AD-ID-DC",
);
export const serviceSupportUser = new TestUser(
    "65F5A723-101D-4E29-B5C3-3A704D229477",
    "cypress service-support",
    "TEST-AD-ID-SS",
);
