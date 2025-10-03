using Dfe.Complete.Application.Contacts.Models;

namespace Dfe.Complete.TagHelpers
{
    public record ContactRadioListModel(
        IEnumerable<ContactDto> Contacts, 
        Guid? SelectedContactId,
        string PropertyName)
    {
        public string Legend { get; set; } = string.Empty;
    }
}
