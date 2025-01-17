using Dfe.Complete.Pages.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.List;

public class AllProjectsViewModel : PageModel
{
    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public PaginationModel Pagination { get; set; } = default!;
    
    internal int PageSize = 20;
}