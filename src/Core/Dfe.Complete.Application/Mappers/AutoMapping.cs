using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Common.Mappers
{
	public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			CreateMap<Project, ProjectDto>();
			CreateMap<ProjectGroup, ProjectGroupDto>();
			CreateMap<User, UserDto>();
			CreateMap<Note, NoteDto>()
				.ForCtorParam(nameof(NoteDto.UserFullName),
					opt => opt.MapFrom(src => src.User.FullName));
		}
	}
}