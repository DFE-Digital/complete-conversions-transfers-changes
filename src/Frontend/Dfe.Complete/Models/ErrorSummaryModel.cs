namespace Dfe.Complete.Models
{
    public class ErrorSummaryModel
    {
        public bool HideKeyInErrorMessage { get; set; }

        public List<string> FieldOrder { get; set; } = new List<string>();
    }
}
