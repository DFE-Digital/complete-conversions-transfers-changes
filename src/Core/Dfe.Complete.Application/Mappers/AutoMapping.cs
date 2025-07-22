using AutoMapper;
using Dfe.AcademiesApi.Client.Contracts;
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
			CreateMap<GiasEstablishment, GiasEstablishmentDto>();
			CreateMap<GiasEstablishment, EstablishmentDto>()
				.ForMember(dest => dest.Ukprn,
					opt => opt.MapFrom(src => src.Ukprn != null ? src.Ukprn.ToString() : null))
				.ForMember(dest => dest.Urn, opt => opt.MapFrom(src => src.Urn != null ? src.Urn.ToString() : null))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.LocalAuthorityCode, opt => opt.MapFrom(src => src.LocalAuthorityCode))
				.ForMember(dest => dest.LocalAuthorityName, opt => opt.MapFrom(src => src.LocalAuthorityName))
				.ForMember(dest => dest.EstablishmentNumber, opt => opt.MapFrom(src => src.EstablishmentNumber))
				.ForMember(dest => dest.StatutoryLowAge,
					opt => opt.MapFrom(src => src.AgeRangeLower.HasValue ? src.AgeRangeLower.Value.ToString() : null))
				.ForMember(dest => dest.StatutoryHighAge,
					opt => opt.MapFrom(src => src.AgeRangeUpper.HasValue ? src.AgeRangeUpper.Value.ToString() : null))
				.ForMember(dest => dest.Diocese, opt => opt.MapFrom(src => new NameAndCodeDto
				{
					Name = src.DioceseName,
					Code = src.DioceseCode
				}))
				.ForMember(dest => dest.EstablishmentType, opt => opt.MapFrom(src => new NameAndCodeDto
				{
					Name = src.TypeName,
					Code = src.TypeCode
				}))
				.ForMember(dest => dest.Gor, opt => opt.MapFrom(src => new NameAndCodeDto
				{
					Name = src.RegionName,
					Code = src.RegionCode
				}))
				.ForMember(dest => dest.PhaseOfEducation, opt => opt.MapFrom(src => new NameAndCodeDto
				{
					Name = src.PhaseName,
					Code = src.PhaseCode
				}))
				.ForMember(dest => dest.ParliamentaryConstituency, opt => opt.MapFrom(src => new NameAndCodeDto
				{
					Name = src.ParliamentaryConstituencyName,
					Code = src.ParliamentaryConstituencyCode
				}))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDto
				{
					Street = src.AddressStreet,
					Additional = src.AddressAdditional,
					Locality = src.AddressLocality,
					Town = src.AddressTown,
					County = src.AddressCounty,
					Postcode = src.AddressPostcode
				}))
				.ForMember(dest => dest.OfstedRating, opt => opt.Ignore())
				.ForMember(dest => dest.OfstedLastInspection, opt => opt.Ignore())
				.ForMember(dest => dest.SchoolCapacity, opt => opt.Ignore())
				.ForMember(dest => dest.Pfi, opt => opt.Ignore())
				.ForMember(dest => dest.Pan, opt => opt.Ignore())
				.ForMember(dest => dest.Deficit, opt => opt.Ignore())
				.ForMember(dest => dest.ViabilityIssue, opt => opt.Ignore())
				.ForMember(dest => dest.GiasLastChangedDate, opt => opt.Ignore())
				.ForMember(dest => dest.NoOfBoys, opt => opt.Ignore())
				.ForMember(dest => dest.NoOfGirls, opt => opt.Ignore())
				.ForMember(dest => dest.SenUnitCapacity, opt => opt.Ignore())
				.ForMember(dest => dest.SenUnitOnRoll, opt => opt.Ignore())
				.ForMember(dest => dest.ReligousEthos, opt => opt.Ignore())
				.ForMember(dest => dest.HeadteacherTitle, opt => opt.Ignore())
				.ForMember(dest => dest.HeadteacherFirstName, opt => opt.Ignore())
				.ForMember(dest => dest.HeadteacherLastName, opt => opt.Ignore())
				.ForMember(dest => dest.HeadteacherPreferredJobTitle, opt => opt.Ignore())
				.ForMember(dest => dest.ReligiousCharacter, opt => opt.Ignore())
				.ForMember(dest => dest.Census, opt => opt.Ignore())
				.ForMember(dest => dest.MisEstablishment, opt => opt.Ignore());
			CreateMap<TransferTasksData, TransferTaskDataDto>();
            CreateMap<SignificantDateHistory, SignificantDateHistoryDto>();
            CreateMap<SignificantDateHistoryReason, SignificantDateHistoryReasonDto>();
		}
	}
}