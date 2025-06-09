namespace Dfe.Complete.Application.Projects.Models;

public record ListTrustsWithProjectsResultModel(string identifier, string trustName, string ukprn, int conversionCount, int transfersCount);