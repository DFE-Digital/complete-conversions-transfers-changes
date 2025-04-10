import { TestUser } from "cypress/constants/TestUser";

export const EnvUrl = "url";
export const EnvApi = "api";
export const EnvUsername = "username";
export const EnvAuthKey = "authKey";
export const EnvTenantId = "tenantId";
export const EnvClientId = "clientId";
export const EnvClientSecret = "clientSecret";
export const EnvCompleteApiClientId = "completeApiClientId";
export const EnvUserAdId = "userAdId";
export const UserAccessToken = "accessToken";
export const ProjectRecordCreator = "projectrecordcreator";
export const Username = "cypress testuser";

// test users
export const cypressUser = new TestUser("cypress testuser", "TEST-AD-ID", "london");
export const rdoLondonUser = new TestUser("cypress rdo-London", "TEST-AD-ID-RDO", "london");
export const regionalCaseworkerUser = new TestUser(
    "cypress regional-casework-services",
    "TEST-AD-ID-RCS",
    "regional_casework_services",
);
export const businessSupportUser = new TestUser("cypress business-support", "TEST-AD-ID-BS", "business_support");
export const dataConsumerUser = new TestUser("cypress data-consumer", "TEST-AD-ID-DC", "data_consumer");
export const serviceSupportUser = new TestUser("cypress service-support", "TEST-AD-ID-SS", "service_support");
export const testUsers: TestUser[] = [
    cypressUser,
    rdoLondonUser,
    regionalCaseworkerUser,
    businessSupportUser,
    dataConsumerUser,
    serviceSupportUser,
];
