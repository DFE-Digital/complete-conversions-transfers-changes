using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Services
{
	public interface ITrustService
	{
        Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(string ukprn);
    }
}
