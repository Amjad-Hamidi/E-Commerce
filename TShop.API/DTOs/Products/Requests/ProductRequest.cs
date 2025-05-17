using TShop.API.Models;

namespace TShop.API.DTOs.Products.Requests
{
    public class ProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public IFormFile MainImg { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        // Foreign Key
        public int CategoryId { get; set; }
        // Foreign Key
        public int BrandId { get; set; }
    }
}
