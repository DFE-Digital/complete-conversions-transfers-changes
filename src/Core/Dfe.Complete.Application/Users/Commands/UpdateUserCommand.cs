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
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Users.Commands
{
    public record UpdateUserCommand(
       UserId Id,
       string FirstName,
       string LastName,
       [InternalEmail] string Email,
       ProjectTeam Team) : IRequest<Result<bool>>;

    internal class UpdateUserCommandHandler(
        ISender sender,
        IUserWriteRepository userWriteRepository,
        ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await sender.Send(new GetUserByIdQuery(request.Id), cancellationToken);

                if (!response.IsSuccess || response.Value == null)
                {
                    logger.LogWarning("{Message} UserId: {UserId}", ErrorMessagesConstants.NotFoundUser, request.Id);
                    return Result<bool>.Failure(string.Format(ErrorMessagesConstants.NotFoundUser, request.Id), ErrorType.NotFound);
                }

                var existingUser = response.Value;
                existingUser.FirstName = request.FirstName;
                existingUser.LastName = request.LastName;
                existingUser.Email = request.Email;
                existingUser.Team = request.Team.ToDescription();

                var userEntity = new User
                {
                    Id = existingUser.Id,
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    Email = existingUser.Email,
                    Team = existingUser.Team
                };

                await userWriteRepository.UpdateUserAsync(userEntity, cancellationToken);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessagesConstants.ExceptionWhileUpdatingUser, request.Id);
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
