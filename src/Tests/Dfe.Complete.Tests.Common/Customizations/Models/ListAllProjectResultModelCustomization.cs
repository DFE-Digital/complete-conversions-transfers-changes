// using AutoFixture;
// using Dfe.Complete.Application.Projects.Model;
// using Dfe.Complete.Domain.Enums;
// using Dfe.Complete.Domain.ValueObjects;
//
// namespace Dfe.Complete.Tests.Common.Customizations.Models;
//
// public class ListAllProjectResultModelCustomization : ICustomization
// {
//     public string? EstablishmentName { get; set; }
//     public ProjectId ProjectId { get; set; }
//     public Urn Urn { get; set; }
//     public DateOnly? ConversionOrTransferDate { get; set; }
//     public ProjectState State { get; set; }
//     public ProjectType? ProjectType { get; set; }
//     public bool IsFormAMAT { get; set; }
//     public string? AssignedToFullName { get; set; }
//     
//     public void Customize(IFixture fixture)
//     {
//         fixture.Customize<ListAllProjectsResultModel>(composer => composer.WithAutoProperties());
//     }
// }