namespace Dfe.Complete.Application.Common.Constants
{
    /// <summary>
    /// Constants for GOV.UK Notify email template keys.
    /// These logical keys map to actual template IDs in configuration.
    /// </summary>
    public static class EmailTemplateKeys
    {
        public const string NewAccountAdded = "NewAccountAdded";
        public const string NewConversionProjectCreated = "NewConversionProjectCreated";
        public const string NewTransferProjectCreated = "NewTransferProjectCreated";
        public const string AssignedNotificationConversion = "AssignedNotificationConversion";
        public const string AssignedNotificationTransfer = "AssignedNotificationTransfer";
    }
}

