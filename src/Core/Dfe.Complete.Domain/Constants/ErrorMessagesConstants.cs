namespace Dfe.Complete.Domain.Constants
{
    public static class ErrorMessagesConstants
    {
        public const string AlreadyExistedLocalAuthorityWithCode = "Already existed local authority with code {0}.";
        public const string CannotUpdateLocalAuthorityAsNotExisted = "Cannot update Local authority as it is not existed.";
        public const string ExceptionWhileUpdatingLocalAuthority = "Error occurred while updating local authority with ID {Id}.";
        public const string CannotDeleteLocalAuthorityAsLinkedToProject = "Cannot delete Local authority as it is linked to a project.";
        public const string NotFoundLocalAuthority = "Local authority with Id {0} not found.";
        public const string ExceptionWhileDeletingLocalAuthority = "Error occurred while deleting local authority with ID {Id}.";
        public const string ExceptionWhileCreatingLocalAuthority = "Error occurred while creating LocalAuthority with code {Code}.";
        public const string CouldNotCreateExternalContact = "Could not create external contact for project {0}.";
        public const string CouldNotUpdateExternalContact = "Could not update external contact for Id {0}.";
        public const string NotFoundExternalContact = "External contact with Id {0} not found.";
        public const string ExceptionGettingExternalContact = "Error occurred while getting external contact with Id {0}.";
    }
}
