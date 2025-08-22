using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Models.ExternalContact
{
    public class ExternalContactDetailsModel
    {
        [Required (ErrorMessage = "Enter a name")]
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsPrimaryProjectContact { get; set; }
    }
}
