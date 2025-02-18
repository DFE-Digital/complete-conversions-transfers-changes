using MediatR;

namespace Dfe.Complete.Application.Common.Models;

public record PaginatedRequest<T>(
    int Page = 0,
    int Count = 20) : IRequest<T>;