using Asp.Versioning;
using Dfe.Complete.Application.Projects.Queries.Csv;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class CsvExportController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [SwaggerResponse(200, "Contents", typeof(File))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> GetConversionCsvByMonthAsync([FromQuery] GetConversionCsvByMonthQuery request, CancellationToken cancellationToken)
        {
            var fileContents = await sender.Send(request, cancellationToken);

            using (var stream = CreateStream(fileContents.Value))
            {
                if (stream == null)
                    return NotFound(); // returns a NotFoundResult with Status404NotFound response.
                
                return File(stream, "application/octet-stream", "{{filename.csv}}");
            }


        }

        private Stream CreateStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
