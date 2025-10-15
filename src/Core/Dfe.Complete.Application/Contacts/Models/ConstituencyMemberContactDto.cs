namespace Dfe.Complete.Application.Contacts.Models
{
    public record ConstituencyMemberContactDto
    {   
        public string DisplayNameWithTitle { get; set; } = null!;        

        public string? Email { get; set; }       
    }
}
