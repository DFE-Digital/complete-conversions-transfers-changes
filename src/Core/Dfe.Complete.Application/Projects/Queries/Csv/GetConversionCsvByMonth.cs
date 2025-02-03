using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces.CsvExport;
using Dfe.Complete.Application.Services.CsvExport;
using Dfe.Complete.Application.Services.TrustCache;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.Csv
{
    public record GetConversionCsvByMonthQuery(int month, int year) : IRequest<Result<string>>;

    public class GetConversionCsvByMonthQueryHandler(IConversionCsvQueryService conversionCsvQueryService,
                                                     ITrustCache trustCache,
                                                     ICSVFileContentGenerator<ConversionCsvModel> generator)
       : IRequestHandler<GetConversionCsvByMonthQuery, Result<string>>
    {
        public async Task<Result<string?>> Handle(GetConversionCsvByMonthQuery request, CancellationToken cancellationToken)
        {
            var result = await conversionCsvQueryService.GetByMonth(request.month, request.year).ToListAsync(cancellationToken);
            if(result?.Count == 0)
            {
                return Result<string?>.Failure("No data found");
            }

            await trustCache.HydrateCache(result.Select(x => x.Project.IncomingTrustUkprn));
            var contents = generator.Generate(result);
            return Result<string?>.Success(contents);
            //return Result<string?>.Success("");
            //    return Result<Project?>.Success(result);
            //}
            //catch (Exception ex)
            //{
            //    return Result<Project?>.Failure(ex.Message);
            //}
        }
    }
}
