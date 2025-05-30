using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Models;
public record OrderProjectQueryBy(OrderProjectByField Field = OrderProjectByField.SignificantDate, OrderByDirection Direction = OrderByDirection.Ascending);
