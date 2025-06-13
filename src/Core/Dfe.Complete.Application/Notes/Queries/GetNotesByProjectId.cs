using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Notes.Queries;

public record GetNotesByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<List<NoteDto>>>;

public class GetNotesByProjectIdQueryHandler(INoteReadRepository noteReadRepository)
    : IRequestHandler<GetNotesByProjectIdQuery, Result<List<NoteDto>>>
{

    public async Task<Result<List<NoteDto>>> Handle(
        GetNotesByProjectIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var notes = await noteReadRepository.GetNotesForProject(request.ProjectId)
                .ToListAsync(cancellationToken);

            var ordered = notes
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return Result<List<NoteDto>>.Success(ordered);
        }
        catch (Exception ex)
        {
            return Result<List<NoteDto>>.Failure(ex.Message);
        }
    }
}
