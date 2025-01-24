using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Services
{
	public interface ITrustService
	{
        Task<IEnumerable<TrustDetailsDto>> GetTrustByUkprn(string ukprn);
    }
}
