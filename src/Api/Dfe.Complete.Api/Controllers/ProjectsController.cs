using Asp.Versioning;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Dfe.Complete.Application.Projects.Commands.RemoveProject;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Queries.SearchProjects;

namespace Dfe.Complete.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ProjectsController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Creates a new conversion project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPost]
        [Route("Create/Conversion")]
        [SwaggerResponse(201, "Project created successfully.", typeof(ProjectId))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateConversionProjectAsync([FromBody] CreateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var projectId = await sender.Send(request, cancellationToken);
            return Created("", projectId);
        }

        /// <summary>
        /// Creates a new Transfer project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPost]
        [Route("Create/Transfer")]
        [SwaggerResponse(201, "Project created successfully.", typeof(ProjectId))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateTransferProjectAsync([FromBody] CreateTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var projectId = await sender.Send(request, cancellationToken);
            return Created("", projectId);
        }

        /// <summary>
        /// Creates a new Form a Mat Conversion project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPost]
        [Route("Create/MatConversion")]
        [SwaggerResponse(201, "Project created successfully.", typeof(ProjectId))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateMatConversionProjectAsync([FromBody] CreateMatConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var projectId = await sender.Send(request, cancellationToken);
            return Created("", projectId);
        }

        /// <summary>
        /// Creates a new Form a Mat Transfer project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPost]
        [Route("Create/MatTransfer")]
        [SwaggerResponse(201, "Project created successfully.", typeof(ProjectId))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CreateMatTransferProjectAsync([FromBody] CreateMatTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var projectId = await sender.Send(request, cancellationToken);
            return Created("", projectId);
        }

        /// <summary>
        /// Gets a Project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [SwaggerResponse(200, "Project", typeof(ProjectDto))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> GetProjectAsync([FromQuery] GetProjectByUrnQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Projects
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsAsync([FromQuery] ListAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Projects related to a specific trust
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/Trust")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsInTrustAsync([FromQuery] ListAllProjectsInTrustQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value?.Projects ?? []);
        }

        /// <summary>
        /// Returns a list of all MATs
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/Mat")]
        [SwaggerResponse(200, "List of all MATs", typeof(List<ListMatResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllMaTsAsync([FromQuery] ListAllMaTsQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value ?? []);
        }
        
        /// <summary>
        /// Returns the number of Projects
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("Count/All")]
        [SwaggerResponse(200, "Project", typeof(int))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> CountAllProjectsAsync([FromQuery] CountAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Projects for a local authority
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/LocalAuthority")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsForLocalAuthorityAsync(
            [FromQuery] ListAllProjectsForLocalAuthorityQuery request,
            CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Regions with project counts
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/Regions")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsByRegionsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsByRegionAsync([FromQuery] ListAllProjectsByRegionQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Projects for a region
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/Region")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsForRegionAsync(
            [FromQuery] ListAllProjectsForRegionQuery request,
            CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(request.Region))
            {
                return BadRequest($"Invalid region \"{request.Region}\" specified");
            }

            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Projects for a team
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/Team")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsForTeamAsync(
            [FromQuery] ListAllProjectsForTeamQuery request,
            CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(request.Team))
            {
                return BadRequest($"Invalid team \"{request.Team}\" specified");
            }

            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Projects for a user
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/User")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsForUserQueryResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> ListAllProjectsForUserAsync(
            [FromQuery] ListAllProjectsForUserQuery request,
            CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);

            if (!project.IsSuccess)
                return project.Error == "User not found."
                    ? BadRequest($"User does not exist for provided {nameof(request.UserAdId)}")
                    : StatusCode(500);

            return Ok(project.Value);
        }

        /// <summary>
        /// Gets the UKPRN for a group reference number.
        /// </summary>
        /// <param name="groupReferenceNumber">The group reference number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet("{groupReferenceNumber}/project_group")]
        [SwaggerResponse(200, "Project Group returned successfully.", typeof(ProjectGroupDto))]
        [SwaggerResponse(400, "Invalid group reference number.")]
        [SwaggerResponse(404, "Project Group not found for the given group reference number.")]
        public async Task<IActionResult> GetProjectGroupByGroupReferenceNumber_Async(string groupReferenceNumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(groupReferenceNumber))
            {
                return BadRequest("Group reference number is required.");
            }

            var request = new GetProjectGroupByGroupReferenceNumberQuery(groupReferenceNumber);
            var ukprn = await sender.Send(request, cancellationToken);

            return Ok(ukprn);
        }

        /// <summary>
        /// Removes project based on URN for test purposes.
        /// </summary>
        /// <param name="urn">Urn to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete]
        [Authorize(Policy = "CanReadWriteUpdateDelete")]
        [SwaggerResponse(204, "Project Group returned successfully.")]
        public async Task<IActionResult> RemoveProject(Urn urn, CancellationToken cancellationToken)
        {
            if (urn == null)
            {
                return BadRequest("Urn is required.");
            }

            var request = new RemoveProjectCommand(urn);
            await sender.Send(request, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Search list of project based on search criteria
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("SearchProjects")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> SearchProjectsAsync(
            [FromQuery] SearchProjectsQuery request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return BadRequest("The SearchTerm field is required.");
            }
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value);
        }

        /// <summary>
        /// Returns a list of Projects by trust reference number
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/TrustRef")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsQueryModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsByTrustRefAsync([FromQuery] ListEstablishmentsInMatQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value?.ProjectModels ?? []);
        }
        /// <summary>
        /// Returns a list of all projects statistics.
        /// </summary> 
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/Statistics")]
        [SwaggerResponse(200, "Project", typeof(ListAllProjectsStatisticsModel))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsStatisticsAsync(CancellationToken cancellationToken)
        {
            var statistics = await sender.Send(new ListAllProjectsStatisticsQuery(), cancellationToken);
            return Ok(statistics.Value);
        } 
        
        /// <summary>
        /// Returns a list of Projects converting to an academy
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanRead")]
        [HttpGet]
        [Route("List/All/Converting")]
        [SwaggerResponse(200, "Project", typeof(List<ListAllProjectsConvertingQueryResultModel>))]
        [SwaggerResponse(400, "Invalid request data.")]
        public async Task<IActionResult> ListAllProjectsConvertingAsync([FromQuery] ListAllProjectsConvertingQuery request, CancellationToken cancellationToken)
        {
            var project = await sender.Send(request, cancellationToken);
            return Ok(project.Value ?? []);
        }
        
        /// <summary>
        /// Updates the Academy URN for a specific project.
        /// </summary>
        /// <param name="request">The update command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [Authorize(Policy = "CanReadWrite")]
        [HttpPatch("project/academy-urn")]
        [SwaggerResponse(204, "Academy URN updated successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(404, "Project not found.")]
        public async Task<IActionResult> UpdateAcademyUrnAsync(
            [FromBody] UpdateAcademyUrnCommand request,
            CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
    }
}
