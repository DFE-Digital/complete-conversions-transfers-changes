namespace Dfe.Complete.Application.Projects.Models;

public record ListTrustsWithProjectsResultModel(string identifier, string trustName, string referenceNumber, int conversionCount, int transfersCount);