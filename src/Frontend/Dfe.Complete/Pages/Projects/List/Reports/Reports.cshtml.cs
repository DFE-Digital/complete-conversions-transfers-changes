using Dfe.Complete.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Pages.Projects.List.Reports;

[Authorize(policy: UserPolicyConstants.CanViewAllProjectsExports)]
public class ReportsViewModel() : AllProjectsModel(ReportsNavigation);