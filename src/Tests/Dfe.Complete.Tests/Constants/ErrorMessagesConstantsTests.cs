using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Tests.Constants
{
    public class ErrorMessagesConstantsTests
    {
        [Fact]
        public void AlreadyExistsProjectGroupWithIdentifier_ShouldReturnCorrectMessage()
        {
            var identifier = "ABC123";
            var expected = $"Project group with identifier '{identifier}' already exists.";
            var actual = string.Format(ErrorMessagesConstants.AlreadyExistsProjectGroupWithIdentifier, identifier);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AlreadyExistsProjectGroupWithUkprn_ShouldReturnCorrectMessage()
        {
            var ukprn = 123456;
            var expected = $"Project group with UKPRN '{ukprn}' already exists.";
            var actual = string.Format(ErrorMessagesConstants.AlreadyExistsProjectGroupWithUkprn, ukprn);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AlreadyExistedLocalAuthorityWithCode_ShouldReturnCorrectMessage()
        {
            var code = "ABC123";
            var expected = $"Already existed local authority with code {code}.";
            var actual = string.Format(ErrorMessagesConstants.AlreadyExistedLocalAuthorityWithCode, code);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CannotUpdateLocalAuthorityAsNotExisted_ShouldReturnCorrectMessage()
        {
            Assert.Equal(
            "Cannot update Local authority as it is not existed.",
            ErrorMessagesConstants.CannotUpdateLocalAuthorityAsNotExisted);
        }

        [Fact]
        public void ExceptionWhileUpdatingLocalAuthority_ShouldContainPlaceholder()
        {
            var id = 42;
            var expected = $"Error occurred while updating local authority with ID {id}.";
            var actual = ErrorMessagesConstants.ExceptionWhileUpdatingLocalAuthority.Replace("{Id}", id.ToString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CannotDeleteLocalAuthorityAsLinkedToProject_ShouldReturnCorrectMessage()
        {
            Assert.Equal(
            "Cannot delete Local authority as it is linked to a project.",
            ErrorMessagesConstants.CannotDeleteLocalAuthorityAsLinkedToProject);
        }

        [Fact]
        public void NotFoundLocalAuthority_ShouldReturnFormattedMessage()
        {
            var id = 99;
            var expected = $"Local authority with Id {id} not found.";
            var actual = string.Format(ErrorMessagesConstants.NotFoundLocalAuthority, id);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ExceptionWhileDeletingLocalAuthority_ShouldReturnFormattedMessage()
        {
            var id = 77;
            var expected = $"Error occurred while deleting local authority with ID {id}.";
            var actual = ErrorMessagesConstants.ExceptionWhileDeletingLocalAuthority.Replace("{Id}", id.ToString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ExceptionWhileCreatingLocalAuthority_ShouldReturnFormattedMessage()
        {
            var code = "XYZ789";
            var expected = $"Error occurred while creating LocalAuthority with code {code}.";
            var actual = ErrorMessagesConstants.ExceptionWhileCreatingLocalAuthority.Replace("{Code}", code);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NotFoundUser_ShouldReturnFormattedMessage()
        {
            var id = "123";
            var expected = $"User with Id 123 not found.";
            var actual = string.Format(ErrorMessagesConstants.NotFoundUser, id);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ExceptionWhileCreatingUser_ShouldReturnFormattedMessage()
        {
            var email = "user@example.com";
            var expected = $"Error occurred while creating User with email {email}.";
            var actual = ErrorMessagesConstants.ExceptionWhileCreatingUser.Replace("{Email}", email);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InvalidGuidException_ShouldReturnFormattedMessage()
        {
            var guid = "a-1-2-3";
            var expected = "a-1-2-3 is not a valid GUID.";
            var actual = string.Format(ErrorMessagesConstants.InvalidGuidException, guid);
            Assert.Equal(expected, actual);
        }
    }
}
