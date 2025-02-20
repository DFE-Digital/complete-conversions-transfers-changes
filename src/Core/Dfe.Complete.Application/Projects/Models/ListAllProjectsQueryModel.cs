using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsQueryModel(Project? Project, GiasEstablishment? Establishment);