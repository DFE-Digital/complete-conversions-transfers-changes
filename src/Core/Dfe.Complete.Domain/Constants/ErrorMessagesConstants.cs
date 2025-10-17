namespace Dfe.Complete.Domain.Constants
{
    public static class ErrorMessagesConstants
    {
        public const string AlreadyExistsUserWithCode = "User with email '{0}' already exists.";
        public const string AlreadyExistedLocalAuthorityWithCode = "Already existed local authority with code {0}.";
        public const string CannotUpdateLocalAuthorityAsNotExisted = "Cannot update Local authority as it is not existed.";
        public const string ExceptionWhileUpdatingLocalAuthority = "Error occurred while updating local authority with ID {Id}.";
        public const string CannotDeleteLocalAuthorityAsLinkedToProject = "Cannot delete Local authority as it is linked to a project.";
        public const string NotFoundUser = "User with Id {0} not found.";
        public const string NotFoundLocalAuthority = "Local authority with Id {0} not found.";
        public const string ExceptionWhileCreatingUser = "Error occurred while creating User with email {Email}.";
        public const string ExceptionWhileUpdatingUser = "Error occurred while updating User with id {Id}.";
        public const string ExceptionWhileDeletingLocalAuthority = "Error occurred while deleting local authority with ID {Id}.";
        public const string ExceptionWhileCreatingLocalAuthority = "Error occurred while creating LocalAuthority with code {Code}.";
        public const string CouldNotCreateExternalContact = "Could not create external contact with project {ProjectId}.";
        public const string CouldNotUpdateExternalContact = "Could not update external contact with Id {Id}.";
        public const string CouldNotDeleteExternalContact = "Could not delete external contact with Id {Id}.";
        public const string NotFoundExternalContact = "External contact with Id {Id} not found.";
        public const string ExceptionGettingExternalContact = "Error occurred while getting external contact with Id {Id}.";
        public const string ExceptionCreatingUser = "Error occurred while creating user with email {0}.";
        public const string InvalidGuidLog = "{Guid} is not a valid GUID.";
        public const string InvalidGuidException = "{0} is not a valid GUID.";
    }
}
