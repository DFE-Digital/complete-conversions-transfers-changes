// using Dfe.Complete.Application.Notes.Queries;
// using Dfe.Complete.Application.Projects.Models;
// using Dfe.Complete.Constants;
// using Dfe.Complete.Domain.Constants;
// using Dfe.Complete.Domain.Enums;
// using Dfe.Complete.Domain.ValueObjects;
// using Dfe.Complete.Extensions;
// using Dfe.Complete.Pages.Projects.TaskList.Tasks;
// using MediatR;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;

// // TODO move the whole lot to the base task model and remove this
// namespace Dfe.Complete.Models;

// public class ProjectTaskNotesModel(ISender sender, IAuthorizationService _authorizationService) : ProjectTaskBaseModel(sender)
// {
//     public IReadOnlyList<NoteDto> Notes { get; private set; } = [];

//     public bool CanAddNotes => Project.State != ProjectState.Deleted && Project.State != ProjectState.Completed && Project.State != ProjectState.DaoRevoked;

//     public bool CanEditNote(UserId noteUserId)
//     {
//         if (Project.State == ProjectState.Completed || noteUserId != User.GetUserId())
//             return false;
//         return true;
//     }

// }