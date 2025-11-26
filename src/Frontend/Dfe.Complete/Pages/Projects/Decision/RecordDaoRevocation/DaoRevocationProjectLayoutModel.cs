using Dfe.Complete.Application.DaoRevoked.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using Dfe.Complete.Services.Interfaces;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Caching.Helpers;
using GovUK.Dfe.CoreLibs.Caching.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Pages.Projects.Decision.RecordDaoRevocation
{
    [ExcludeFromCodeCoverage]
    public abstract class DaoRevocationProjectLayoutModel(ISender sender, ILogger logger, ICacheService<IMemoryCacheType> cacheService, IProjectPermissionService projectPermissionService) : ProjectLayoutModel(sender, logger, RecordDaoRevocationNavigation)
    {
        protected readonly IProjectPermissionService ProjectPermissionService = projectPermissionService;
        protected string CacheKey => $"DaoRevocation_{CacheKeyHelper.GenerateHashedCacheKey(ProjectId)}";

        protected virtual async Task<IActionResult?> CheckDaoRevocationPermissionAsync()
        {
            if (!ProjectPermissionService.UserCanDaoRevocation(Project, User))
            {
                TempData.SetNotification(
                    NotificationType.Error,
                    "Important",
                    "You are not authorised to perform this action."
                );
                return Redirect(FormatRouteWithProjectId(RouteConstants.Project));
            }
            return null;
        }

        protected IActionResult ReturnPage(RecordDaoRevocationDecisionCommand decision)
            => decision.ReasonNotes?.Count == 0 ? RedirectToDaoRevocationPage() : Page();

        protected async Task<RecordDaoRevocationDecisionCommand> GetCachedDecisionAsync()
        {
            return await cacheService.GetOrAddAsync(CacheKey, () =>
            {
                var command = new RecordDaoRevocationDecisionCommand(new ProjectId(new Guid(ProjectId)));
                return Task.FromResult(command);
            }, string.Empty);
        }

        protected async Task UpdateCacheAsync(RecordDaoRevocationDecisionCommand decison)
        {
            cacheService.Remove(CacheKey);
            await cacheService.GetOrAddAsync(CacheKey, () =>
            {
                return Task.FromResult(decison);
            }, string.Empty);
        }
        public RedirectResult RedirectToDaoRevocationPage()
            => RedirectToDaoRevocationRoute(RouteConstants.ProjectDaoRevocation);

        public string GetDaoRevocationCheckPage()
            => FormatRouteWithProjectId(RouteConstants.ProjectDaoRevocationCheck);

        protected RedirectResult RedirectToDaoRevocationRoute(string route)
            => Redirect(FormatRouteWithProjectId(route));

        protected static void PopulateOptions(List<DaoRevokedReason> reasons)
        {
            reasons.Add(DaoRevokedReason.SchoolRatedGoodOrOutstanding);
            reasons.Add(DaoRevokedReason.SafeguardingConcernsAddressed);
            reasons.Add(DaoRevokedReason.SchoolClosedOrClosing);
            reasons.Add(DaoRevokedReason.ChangeToGovernmentPolicy);
        }
        protected static void ValidateReasons(IFormCollection formValues, List<DaoRevokedReason> reasons, Dictionary<DaoRevokedReason, string> reasonNotes, IErrorService errorService, ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, string>();
            foreach (var reason in reasons)
            {
                var reasonKey = $"dao_revoked_reasons[{reason.ToDescription()}]";
                var noteKey = $"dao_revoked_reasons[{reason.ToDescription()}_note]";

                var isChecked = formValues.TryGetValue(reasonKey, out var selected) && selected == "1";
                if (!isChecked)
                    continue;

                var hasNote = formValues.TryGetValue(noteKey, out var note);
                var isNoteValid = hasNote && !string.IsNullOrWhiteSpace(note);

                if (isNoteValid)
                {
                    reasonNotes[reason] = note!;
                }
                else
                {
                    errors.Add($"{reason.ToDescription()}_note", ValidationConstants.MustProvideDetails);
                }
            }

            if (reasonNotes.Count == 0 && errors.Count == 0)
            {
                errors.Add("select-dao-revoked-reason", ValidationConstants.ChooseAtLeastOneReason);
            }

            foreach (var error in errors.Distinct())
            {
                modelState.AddModelError(error.Key, error.Value);
            }

            errorService.AddErrors(modelState);
        }
    }
}
