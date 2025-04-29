import { TestUser } from "cypress/constants/TestUser";
import { UserRoles as Role } from "cypress/constants/UserRoles";

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
export const cypressUser = new TestUser("cypress testuser", "TEST-AD-ID", [
    Role.regionalDeliveryOfficer,
    Role.addNewProject,
    Role.assignToProject,
]);
export const rdoLondonUser = new TestUser("cypress rdo-london", "TEST-AD-ID-RDO", [
    Role.regionalDeliveryOfficer,
    Role.addNewProject,
    Role.assignToProject,
]);
export const rdoTeamLeaderUser = new TestUser("cypress rdo-team-leader", "TEST-AD-ID-RDO-TL", [
    Role.regionalDeliveryOfficer,
    Role.manageTeam,
    Role.addNewProject,
    Role.assignToProject,
]);
export const regionalCaseworkerUser = new TestUser("cypress regional-casework-services", "TEST-AD-ID-RCS", [
    Role.regionalCaseworkerServices,
    Role.addNewProject,
    Role.assignToProject,
]);
export const regionalCaseworkerTeamLeaderUser = new TestUser("cypress rcs-team-leader", "TEST-AD-ID-RCS-TL", [
    Role.regionalCaseworkerServices,
    Role.manageTeam,
    Role.addNewProject,
    Role.assignToProject,
]);
export const businessSupportUser = new TestUser("cypress business-support", "TEST-AD-ID-BS", [Role.businessSupport]);
export const dataConsumerUser = new TestUser("cypress data-consumers", "TEST-AD-ID-DC", [Role.dataConsumers]);
export const serviceSupportUser = new TestUser("cypress service-support", "TEST-AD-ID-SS", [
    Role.serviceSupport,
    Role.addNewProject,
    Role.manageUserAccounts,
    Role.manageConversionURNs,
    Role.manageLocalAuthorities,
]);
