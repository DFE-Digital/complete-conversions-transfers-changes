using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.ProjectGroups.Interfaces;
using Dfe.Complete.Application.ProjectGroups.Queries.QueryFilters;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.ProjectGroups.Commands;

// TODO - check trust exists? Currently returning 400 for duplicate GRN...? Rename file to Command
public record CreateProjectGroupCommand(
   [Required]
   [GroupReferenceNumber]
   string GroupReferenceNumber,
   [Required]
   [Ukprn] Ukprn Ukprn
   ) : IRequest<Result<ProjectGroupId>>;

internal class CreateProjectGroupCommandHandler(
   IProjectGroupWriteRepository projectGroupWriteRepository,
   IProjectGroupReadRepository projectGroupReadRepository,
   ILogger<CreateProjectGroupCommandHandler> logger
) : IRequestHandler<CreateProjectGroupCommand, Result<ProjectGroupId>>
{
   public async Task<Result<ProjectGroupId>> Handle(CreateProjectGroupCommand request, CancellationToken cancellationToken)
   {
       try
       {
            if(await new ProjectGroupIdentifierQuery(request.GroupReferenceNumber).Apply(projectGroupReadRepository.ProjectGroups).FirstOrDefaultAsync(cancellationToken) is not null)
                throw new AlreadyExistsException(string.Format(ErrorMessagesConstants.AlreadyExistsProjectGroupWithIdentifier, request.GroupReferenceNumber));

            if(await new ProjectGroupUkprnQuery(request.Ukprn).Apply(projectGroupReadRepository.ProjectGroups).FirstOrDefaultAsync(cancellationToken) is not null)
                throw new AlreadyExistsException(string.Format(ErrorMessagesConstants.AlreadyExistsProjectGroupWithUkprn, request.Ukprn));

           var now = DateTime.UtcNow;
           var group = new ProjectGroup()
           {
               Id = new ProjectGroupId(Guid.NewGuid()),
               GroupIdentifier = request.GroupReferenceNumber,
               TrustUkprn = request.Ukprn,
               CreatedAt = now,
               UpdatedAt = now
           };

           await projectGroupWriteRepository.CreateProjectGroupAsync(group, cancellationToken);

           return Result<ProjectGroupId>.Success(group.Id);
       }
       catch (Exception ex)
       {
           logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(CreateProjectGroupCommandHandler), request);
           return Result<ProjectGroupId>.Failure($"Could not create project group: {ex.Message}");
       }
   }
}
