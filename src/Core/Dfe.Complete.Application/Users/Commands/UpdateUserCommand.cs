using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Queries.QueryFilters;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Domain.Validators;

namespace Dfe.Complete.Application.Users.Commands
{
    public record UpdateUserCommand(
       UserId Id,
       string FirstName,
       string LastName,
       [InternalEmail] string Email,
       ProjectTeam Team) : IRequest<Result<bool>>;

    internal class UpdateUserCommandHandler(
       IUserReadRepository userReadRepository,
       IUserWriteRepository userWriteRepository,
       ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await new UserIdQuery(request.Id).Apply(userReadRepository.Users).FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(string.Format(ErrorMessagesConstants.NotFoundUser, request.Id.Value));

                existingUser.FirstName = request.FirstName;
                existingUser.LastName = request.LastName;
                existingUser.Email = request.Email;
                existingUser.Team = request.Team.ToDescription();

                await userWriteRepository.UpdateUserAsync(existingUser, cancellationToken);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessagesConstants.ExceptionWhileUpdatingUser, request.Id);
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return Result<bool>.Failure(errorMessage);
            }
        }
    }
}
