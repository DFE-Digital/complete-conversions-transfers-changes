using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Services.TrustService
{
    public interface ITrustService
    {
        Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(string ukprn);
        Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(IEnumerable<string> ukprns);
    }
}
