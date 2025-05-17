using TShop.API.Models;

namespace TShop.API.DTOs.Users
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
