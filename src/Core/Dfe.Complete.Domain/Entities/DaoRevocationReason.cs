using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Entities;

public class DaoRevocationReason : IEntity<DaoRevocationReasonId>
{
    public DaoRevocationReasonId Id { get; set; }

    public DaoRevocationId? DaoRevocationId { get; set; }

    public string? ReasonType { get; set; }
}
