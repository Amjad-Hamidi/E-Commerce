using System.ComponentModel.DataAnnotations;
using TShop.API.Models;
using TShop.API.Validations;

namespace TShop.API.DTOs.Registers
{
    public class RegisterRequest
    {
        [MinLength(3)]
        public string FirstName { get; set; }
        [MinLength(5)]
        public string LastName { get; set; }
        [MinLength(6)]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public ApplicationUserGender Gender { get; set; }
        [OverYears(16)]   //[Over18Years]
        public DateTime BirthDate { get; set; }
    }
}
