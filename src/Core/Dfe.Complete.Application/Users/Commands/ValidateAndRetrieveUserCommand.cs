using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Dfe.Complete.Application.Users.Commands
{
    public record ValidateAndRetrieveUserCommand(
        ClaimsPrincipal Principal,
        string UserId) : IRequest<Result<User?>>;

    internal class ValidateAndRetrieveUserCommandHandler(
        ICompleteRepository<User> userRepository,
        ILogger<ValidateAndRetrieveUserCommandHandler> logger) : IRequestHandler<ValidateAndRetrieveUserCommand, Result<User?>>
    {
        public async Task<Result<User?>> Handle(ValidateAndRetrieveUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Try to find user by OID first
                var userRecord = await userRepository.FindAsync(u => u.EntraUserObjectId == request.UserId, cancellationToken);
                var email = request.Principal.FindFirst(CustomClaimTypeConstants.PreferredUsername)?.Value;

                // If the email doesn't match (claims vs DB) - reject.
                // Persons name probably changed and there is probably an orphaned record. Contact service support
                if (userRecord != null && !string.Equals(userRecord.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    var errorMessage = $"Duplicate account detected. Your current email ({email}) doesn't match the email associated with your account. This may indicate a duplicate account. Please contact service support";
                    logger.LogWarning("Email mismatch for user {UserId}. DB email: {DbEmail}, Claims email: {ClaimsEmail}", 
                        request.UserId, userRecord.Email, email);
                    throw new InvalidOperationException(errorMessage);
                }

                // If there was no OID match but there was an email match, this is probably first login. 
                if (userRecord == null && !string.IsNullOrEmpty(email))
                {
                    userRecord = await userRepository.FindAsync(u => u.Email == email);
                    if (userRecord != null)
                    {
                        logger.LogInformation("Updating user {UserId} with Entra Object ID {EntraId} for first login", 
                            userRecord.Id.Value, request.UserId);
                        userRecord.EntraUserObjectId = request.UserId;
                        await userRepository.UpdateAsync(userRecord);
                    }
                }

                return Result<User?>.Success(userRecord);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error validating and retrieving user with ID {UserId}", request.UserId);
                return Result<User?>.Failure(ex.Message);
            }
        }
    }
}