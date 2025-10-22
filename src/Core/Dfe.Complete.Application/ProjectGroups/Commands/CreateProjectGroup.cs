//using Dfe.Complete.Application.Common.Models;
//using Dfe.Complete.Application.ProjectGroups.Interfaces;
//using Dfe.Complete.Domain.Entities;
//using Dfe.Complete.Domain.Enums;
//using Dfe.Complete.Domain.ValueObjects;
//using GovUK.Dfe.CoreLibs.Utilities.Extensions;
//using MediatR;
//using Microsoft.Extensions.Logging;

//namespace Dfe.Complete.Application.ProjectGroups.Commands;

//public record CreateProjectGroupCommand(
//    [Required]
//    [GroupReferenceNumber]
//    string GroupReferenceNumber,
//    [Required]
//    [Ukprn] int Ukprn
//    ) : IRequest<Result<ProjectGroupId>>;

//public class CreateProjectGroupCommandHandler(
//    IProjectGroupWriteRepository _repo,
//    ILogger<CreateProjectGroupCommandHandler> logger
//) : IRequestHandler<CreateProjectGroupCommand, Result<ProjectGroupId>>
//{
//    public async Task<Result<ProjectGroupId>> Handle(CreateProjectGroupCommand request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var now = DateTime.UtcNow;
//            var group = new ProjectGroup()
//            {
//                Id = new ProjectGroupId(Guid.NewGuid()),
//                GroupIdentifier = request.GroupReferenceNumber,
//                TrustUkprn = request.Ukprn,
//                CreatedAt = now,
//                UpdatedAt = now
//            };

//            await _repo.CreateProjectGroupAsync(group, cancellationToken);

//            return Result<ProjectGroupId>.Success(group.Id);
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(CreateProjectGroupCommandHandler), request);
//            return Result<ProjectGroupId>.Failure($"Could not create group for project {request.ProjectId.Value}: {ex.Message}");
//        }
//    }
//}
