using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Application.Projects.Services;

public interface IHandoverProjectService
{
    Task<UserId> GetOrCreateUserAsync(UserDto userDto, CancellationToken cancellationToken);
    Task<Project?> FindExistingProjectAsync(int urn, CancellationToken cancellationToken);
    Task SaveProjectAndTaskAsync(Project project, ConversionTasksData conversionTask, CancellationToken cancellationToken);
    ConversionTasksData CreateConversionTaskAsync();
}
