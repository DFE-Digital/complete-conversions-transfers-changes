namespace Dfe.Complete.Pages.Pagination
{
    public class PaginationModel
    {
        public PaginationModel(string url, int pageNumber, int recordCount, int pageSize, string? elementIdPrefix = null)
        {
            Url = url;
            PageNumber = pageNumber;
            RecordCount = recordCount;
            TotalPages = (int)Math.Ceiling((double)recordCount / pageSize);
            if (pageNumber > 1)
            {
                HasPrevious = true;
                Previous = pageNumber - 1;
            }
            else
            {
                HasPrevious = false;
            }
            
            if (pageNumber < TotalPages)
            {
                HasNext = true;
                Next = pageNumber + 1;
            }
            else
            {
                HasNext = false;
            }

            ElementIdPrefix = elementIdPrefix;
        }
        public string Url { get; set; }

        public bool HasNext { get; set; }

        public bool HasPrevious { get; set; }

        public int TotalPages { get; set; }

        public int PageNumber { get; set; }

        public int? Next { get; set; }

        public int? Previous { get; set; }

        public int RecordCount { get; set; }

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
        public string? ElementIdPrefix { get; set; }
    }
}
