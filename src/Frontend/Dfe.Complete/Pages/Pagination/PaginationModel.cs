namespace Dfe.Complete.Pages.Pagination;

public class PaginationModel
{
    public PaginationModel(string url, int currentPageNumber, int recordCount, int pageSize, string? elementIdPrefix = null)
    {
        Url = url;
        CurrentPageNumber = currentPageNumber;
        RecordCount = recordCount;
        TotalPages = (int)Math.Ceiling((double)recordCount / pageSize);
        IsOutOfRangePage = currentPageNumber > TotalPages || currentPageNumber < 1;
        if (currentPageNumber > 1)
        {
            HasPrevious = true;
            Previous = currentPageNumber - 1;
        }
        else
        {
            HasPrevious = false;
        }

        if (currentPageNumber < TotalPages)
        {
            HasNext = true;
            Next = currentPageNumber + 1;
        }
        else
        {
            HasNext = false;
        }

        ElementIdPrefix = elementIdPrefix;

        PagesToDisplay = new List<int>();

        if (currentPageNumber != 1)
            PagesToDisplay.Add(1);
        if (Previous is > 1)
            PagesToDisplay.Add(Previous.Value);
        PagesToDisplay.Add(currentPageNumber);
        if (Next < TotalPages)
            PagesToDisplay.Add(Next.Value);
        if (currentPageNumber != TotalPages)
            PagesToDisplay.Add(TotalPages);
    }

    public List<int> PagesToDisplay { get; init; }
    public string Url { get; init; }

    public bool HasNext { get; init; }

    public bool HasPrevious { get; init; }

    public int TotalPages { get; init; }

    public int CurrentPageNumber { get; init; }

    public int? Next { get; init; }

    public int? Previous { get; init; }

    public int RecordCount { get; init; }

    /// <summary>
    /// The ID of the container that has the content to be changed
    /// This is for partial page reloads when pagination is invoked
    /// When we only want to refresh the content container, not the entire page
    /// </summary>
    // public string ContentContainerId { get; set; }

    /// <summary>
    /// Prefix so that we can have multiple pagination elements on screen
    /// Ensures we can uniquely identify the pagination for separate content containers
    /// </summary>
    public string? ElementIdPrefix { get; init; }

    public string Prefix => !string.IsNullOrEmpty(ElementIdPrefix) ? ElementIdPrefix : "";
    public string? NextPageLink => Next.HasValue ? SetUrl(Url, Next.Value) : null;
    public string? PreviousPageLink => Previous.HasValue ? SetUrl(Url, Previous.Value) : null;
    public string PaginationContainerId => $"{Prefix}pagination-container";
    public string NextButtonId => $"{Prefix}next-page";
    public string PreviousButtonId => $"{Prefix}previous-page";
    public static string SetUrl(string url, int pageNumber) => $"{url}{(url.Contains('?') ? "&" : "?")}page={pageNumber}";

    public bool IsOutOfRangePage { get; set; }
}