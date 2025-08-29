using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Models.ExternalContact
{
    public class OtherExternalContactInputModel : ExternalContactInputModel
    {  
        [Required(ErrorMessage = "Enter a role")]
        public string Role { get; set; }

        public string? Organisation { get; set; }

        public string SelectedExternalContactType { get; set; } = ExternalContactType.Other.ToDescription();

        public ExternalContactType[]? ContactTypeRadioOptions { get; set; }
    }
}
