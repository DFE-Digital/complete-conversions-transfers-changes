﻿using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateAssignedTeamCommand(
    ProjectId ProjectId,
    ProjectTeam? AssignedTeam
) : IRequest;

public class UpdateAssignedTeam(
    ICompleteRepository<Project> projectRepository)
    : IRequestHandler<UpdateAssignedTeamCommand>
{
    public async Task Handle(UpdateAssignedTeamCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken);
        project.Team = request.AssignedTeam;
        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}