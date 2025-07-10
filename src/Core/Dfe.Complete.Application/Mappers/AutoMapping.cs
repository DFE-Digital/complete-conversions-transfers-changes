﻿using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Common.Mappers
{
    public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			CreateMap<Project, ProjectDto>();
            CreateMap<GiasEstablishment, GiasEstablishmentDto>();
            CreateMap<ProjectGroup, ProjectGroupDto>();
            CreateMap<TransferTasksData, TransferTaskDataDto>();
            CreateMap<User, UserDto>();
        }
	}
}