namespace TShop.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string MainImg { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }

        
        // Foreign Key
        public int CategoryId { get; set; }
        // Navigation Property
        public Category Category { get; set; }
        // Foreign Key
        public int BrandId { get; set; }
        // Navigation Property
        public Brand Brand { get; set; }
    }
}
