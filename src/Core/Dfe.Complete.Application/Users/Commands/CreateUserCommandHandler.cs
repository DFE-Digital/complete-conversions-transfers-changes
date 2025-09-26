using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Queries.QueryFilters;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Users.Commands
{
    public record CreateUserCommand(
       string FirstName,
       string LastName,
       string Email,
       ProjectTeam Team) : IRequest<Result<UserId?>>;

    internal class CreateUserCommandHandler(
       IUserReadRepository userReadRepository,
       IUserWriteRepository userWriteRepository,
       ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand, Result<UserId?>>
    {
        public async Task<Result<UserId?>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await new UserEmailQuery(request.Email).Apply(userReadRepository.Users).FirstOrDefaultAsync(cancellationToken);
                if (existingUser != null)
                    throw new AlreadyExistsException(string.Format(ErrorMessagesConstants.AlreadyExistsUserWithCode, request.Email));

                var user = User.Create(new UserId(Guid.NewGuid()), request.Email, request.FirstName, request.LastName, request.Team.ToDescription());

                await userWriteRepository.CreateUserAsync(user, cancellationToken);
                return Result<UserId?>.Success(user.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessagesConstants.ExceptionWhileCreatingUser, request.Email);
                return Result<UserId?>.Failure(ex.Message);
            }
        }
    }
}
