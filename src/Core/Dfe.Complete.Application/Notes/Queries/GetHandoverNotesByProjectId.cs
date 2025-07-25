using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using AutoMapper;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Application.Notes.Queries
{
    public record GetHandoverNotesByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<List<NoteDto>>>;

    public class GetHandoverNotesByProjectIdQueryHandler(
        IMapper mapper,
        ICompleteRepository<Note> noteRepository
    ) : IRequestHandler<GetHandoverNotesByProjectIdQuery, Result<List<NoteDto>>>
    {
        public async Task<Result<List<NoteDto>>> Handle(GetHandoverNotesByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var result = await noteRepository.FetchAsync(p => p.ProjectId == request.ProjectId && p.TaskIdentifier == NoteTaskIdentifier.Handover.ToDescription());

            var dtos = mapper.Map<List<NoteDto>>(result);

            return Result<List<NoteDto>>.Success(dtos);
        }
    }
}
