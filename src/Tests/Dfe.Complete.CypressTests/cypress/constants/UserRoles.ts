export const UserRoles = {
    rdo: "RegionalDeliveryOfficer",
    rcs: "RegionalCaseworkServices",
    rcsTeamLead: "RegionalCaseworkServices.TeamLead",
    dataConsumer: "Data.Consumer",
    serviceSupport: "Support.Service",
} as const;

export type UserRole = (typeof UserRoles)[keyof typeof UserRoles];
