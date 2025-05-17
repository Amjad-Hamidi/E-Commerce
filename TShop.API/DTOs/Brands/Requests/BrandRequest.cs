using System.ComponentModel.DataAnnotations;

namespace TShop.API.DTOs.Brands.Requests
{
    public class BrandRequest
    {
        [Required(ErrorMessage = "Name is required!!!")]
        [MinLength(2)]
        [MaxLength(15)] 
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
