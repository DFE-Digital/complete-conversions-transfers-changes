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

// test users
export const cypressUser = new TestUser("cypress testuser", "TEST-AD-ID", "london");
export const rdoLondonUser = new TestUser("cypress rdo-london", "TEST-AD-ID-RDO", "london");
export const rdoTeamLeaderUser = new TestUser("cypress rdo-team-leader", "TEST-AD-ID-RDO-TL", "london", 1);
export const regionalCaseworkerUser = new TestUser(
    "cypress regional-casework-services",
    "TEST-AD-ID-RCS",
    "regional_casework_services",
);
export const regionalCaseworkerTeamLeaderUser = new TestUser(
    "cypress rcs-team-leader",
    "TEST-AD-ID-RCS-TL",
    "regional_casework_services",
    1,
);
export const businessSupportUser = new TestUser(
    "cypress business-support",
    "TEST-AD-ID-BS",
    "business_support",
    0,
    0,
    0,
    0,
    0,
);
export const dataConsumerUser = new TestUser(
    "cypress data-consumers",
    "TEST-AD-ID-DC",
    "data_consumers",
    0,
    0,
    0,
    0,
    0,
);
export const serviceSupportUser = new TestUser(
    "cypress service-support",
    "TEST-AD-ID-SS",
    "service_support",
    0,
    0,
    1,
    1,
    1,
);
export const testUsers: TestUser[] = [
    cypressUser,
    rdoLondonUser,
    rdoTeamLeaderUser,
    regionalCaseworkerUser,
    regionalCaseworkerTeamLeaderUser,
    businessSupportUser,
    dataConsumerUser,
    serviceSupportUser,
];
