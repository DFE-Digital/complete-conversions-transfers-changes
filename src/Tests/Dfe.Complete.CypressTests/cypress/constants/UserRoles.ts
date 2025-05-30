export const UserRoles = {
    manageTeam: "manage_team",
    addNewProject: "add_new_project",
    assignToProject: "assign_to_project",
    manageUserAccounts: "manage_user_accounts",
    manageConversionURNs: "manage_conversion_urns",
    manageLocalAuthorities: "manage_local_authorities",

    // teams
    regionalDeliveryOfficer: "regional_delivery_officer",
    regionalCaseworkerServices: "regional_casework_services",
    serviceSupport: "service_support",
    businessSupport: "business_support",
    dataConsumers: "data_consumers",
} as const;

export type UserRole = (typeof UserRoles)[keyof typeof UserRoles];
