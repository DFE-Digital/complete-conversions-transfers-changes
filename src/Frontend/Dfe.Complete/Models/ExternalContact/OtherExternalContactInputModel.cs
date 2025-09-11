using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Models.ExternalContact
{
    public class OtherExternalContactInputModel : ExternalContactInputModel
    {   
        public required string Role { get; set; }

        [BindProperty(Name = "organisation-solicitor")]
        public string? OrganisationSolicitor { get; set; }

        [BindProperty(Name = "organisation-diocese")]
        public string? OrganisationDiocese { get; set; }

        [BindProperty(Name = "organisation-other")]
        public string? OrganisationOther { get; set; }

        public string SelectedExternalContactType { get; set; } = ExternalContactType.Other.ToDescription();

        public ExternalContactType[]? ContactTypeRadioOptions { get; set; }
    }
}
