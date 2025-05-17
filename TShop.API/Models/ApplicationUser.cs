using Microsoft.AspNetCore.Identity;

namespace TShop.API.Models
{
    public enum ApplicationUserGender
    {
        Male,Female
    }
    public class ApplicationUser : IdentityUser
    {
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ApplicationUserGender Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
