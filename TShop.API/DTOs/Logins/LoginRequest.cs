using System.ComponentModel.DataAnnotations;

namespace TShop.API.DTOs.Logins
{
    public class LoginRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
