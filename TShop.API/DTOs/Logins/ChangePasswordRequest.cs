using System.ComponentModel.DataAnnotations;

namespace TShop.API.DTOs.Logins
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        [Compare(nameof(NewPassword), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
