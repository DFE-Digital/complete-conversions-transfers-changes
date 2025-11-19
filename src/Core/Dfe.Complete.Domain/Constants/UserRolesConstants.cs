namespace Dfe.Complete.Domain.Constants;

/// <summary>
/// Contains constants for app role values that are expected to be present in user claims.
/// These values correspond to app roles defined in the Azure AD app registration and are
/// used to determine user permissions and access levels within the application.
/// 
/// App roles are assigned to users through Azure AD and appear in the user's JWT token
/// as role claims when they authenticate. The application uses these role values to
/// make authorization decisions.
/// </summary>
public static class UserRolesConstants
{
    public const string RegionalDeliveryOfficer = "regional_delivery_officer";

    // Teams
    public const string RegionalCaseworkServicesTeamLead = "RegionalCaseworkServices.TeamLead";
    public const string RegionalCaseworkServices = "regional_casework_services";
    public const string ServiceSupport = "service_support";
    public const string DataConsumers = "Data.Consumer";
}

