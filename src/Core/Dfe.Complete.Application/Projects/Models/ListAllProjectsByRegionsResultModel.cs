using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsByRegionsResultModel(Region Region, int ConversionsCount, int TransfersCount);