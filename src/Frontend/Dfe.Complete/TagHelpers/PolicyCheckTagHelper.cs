using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Complete.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-policy")]
    public class PolicyCheckTagHelper(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor) : TagHelper
    {
        private readonly IAuthorizationService _authorizationService = authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        [HtmlAttributeName("asp-policy")]
        public required string Policy { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                output.SuppressOutput();
                return;
            }

            var authorized = await _authorizationService.AuthorizeAsync(user, Policy);
            if (!authorized.Succeeded)
                output.SuppressOutput();
        }
    }
}