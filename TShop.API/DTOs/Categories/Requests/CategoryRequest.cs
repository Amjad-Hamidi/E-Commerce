using System.ComponentModel.DataAnnotations;

namespace TShop.API.DTOs.Categories.Requests
{
    public class CategoryRequest
    {
        [Required(ErrorMessage ="Name is required!!!")]
        [MinLength(2)]
        [MaxLength(5)]
        // [AllowedValues("Electronics", "Clothes", "Shoes", "Books")] // القيم المسموح ارسالها
        [DeniedValues("Food", "Drinks", "Furniture")] // القيم الممنوع ارسالها  
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
