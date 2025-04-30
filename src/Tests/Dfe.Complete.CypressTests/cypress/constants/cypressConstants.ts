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
export const cypressUser = new TestUser("cypress testuser", "TEST-AD-ID");
export const rdoLondonUser = new TestUser("cypress rdo-london", "TEST-AD-ID-RDO");
export const rdoTeamLeaderUser = new TestUser("cypress rdo-team-leader", "TEST-AD-ID-RDO-TL");
export const regionalCaseworkerUser = new TestUser("cypress regional-casework-services", "TEST-AD-ID-RCS");
export const regionalCaseworkerTeamLeaderUser = new TestUser("cypress rcs-team-leader", "TEST-AD-ID-RCS-TL");
export const businessSupportUser = new TestUser("cypress business-support", "TEST-AD-ID-BS");
export const dataConsumerUser = new TestUser("cypress data-consumers", "TEST-AD-ID-DC");
export const serviceSupportUser = new TestUser("cypress service-support", "TEST-AD-ID-SS");
