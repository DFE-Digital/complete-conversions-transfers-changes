namespace Dfe.Complete.Application.Projects.Model;

public record ListTrustsWithProjectsResultModel(string ukprn, string trustName, string referenceNumber, int conversionCount, int transfersCount);