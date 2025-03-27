using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

public record CreateMatConversionProjectCommand(
    Urn Urn,
    string NewTrustName,
    string NewTrustReferenceNumber,
    DateOnly SignificantDate,
    bool IsSignificantDateProvisional,
    bool IsDueTo2Ri,
    bool HasAcademyOrderBeenIssued,
    DateOnly AdvisoryBoardDate,
    string AdvisoryBoardConditions,
    string EstablishmentSharepointLink,
    string IncomingTrustSharepointLink,
    bool HandingOverToRegionalCaseworkService,
    string? HandoverComments,
    string? UserAdId) : IRequest<ProjectId>;

 public class CreateMatConversionProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskRepository,
        ICompleteRepository<GiasEstablishment> establishmentRepository,
        ISender sender)
        : IRequestHandler<CreateMatConversionProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateMatConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var localAuthorityIdRequest = await sender.Send(new GetLocalAuthorityBySchoolUrnQuery(request.Urn.Value),
            cancellationToken);

            if (!localAuthorityIdRequest.IsSuccess || localAuthorityIdRequest.Value?.LocalAuthorityId == null)
                throw new NotFoundException($"No Local authority could be found via Establishments for School Urn: {request.Urn.Value}.", nameof(request.Urn), innerException: new Exception(localAuthorityIdRequest.Error));
            
            var establishment = await establishmentRepository.FindAsync(giasEstablishment => giasEstablishment.Urn == request.Urn, cancellationToken);

            if (establishment is null)
            {
                throw new NotFoundException($"No establishment could be found for Urn: {request.Urn.Value}.", nameof(request.Urn));
            }
            
            var region = establishment.RegionCode?.ToEnumFromChar<Region>();

            var createdAt = DateTime.UtcNow;
            var conversionTaskId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());

            var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);
            
            Result<UserDto?>? userRequest = null;
            ProjectTeam team;
            DateTime? assignedAt = null;
            UserDto? projectUser = null;
            UserId? projectUserAssignedToId = null;

            if (request.HandingOverToRegionalCaseworkService)
            {
                team = ProjectTeam.RegionalCaseWorkerServices;
            }
            else
            {
                if (!string.IsNullOrEmpty(request.UserAdId))
                    userRequest = await sender.Send(new GetUserByAdIdQuery(request.UserAdId), cancellationToken);

                if (userRequest is not { IsSuccess: true } || userRequest.Value is null)
                    throw new NotFoundException("No user found.", innerException: new Exception(userRequest?.Error));
            
                projectUser = userRequest.Value;

                team = (projectUser?.Team).FromDescription<ProjectTeam>();
                assignedAt = DateTime.UtcNow;
                projectUserAssignedToId = projectUser?.Id;
            }

            var project = Project.CreateMatConversionProject(
                projectId,
                request.Urn,
                createdAt,
                updatedAt: createdAt,
                TaskType.Conversion,
                ProjectType.Conversion,
                conversionTaskId,
                region,
                team,
                projectUser?.Id,
                projectUserAssignedToId,
                assignedAt,
                request.EstablishmentSharepointLink,
                request.IncomingTrustSharepointLink,
                request.AdvisoryBoardDate,
                request.AdvisoryBoardConditions,
                request.SignificantDate,
                request.IsSignificantDateProvisional,
                request.IsDueTo2Ri,
                request.NewTrustName,
                request.NewTrustReferenceNumber,
                request.HasAcademyOrderBeenIssued, 
                request.HandoverComments, 
                localAuthorityIdRequest.Value.LocalAuthorityId.Value);

            await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }