namespace TShop.API.DTOs.Products.Responses
{
    public class ProductResponse
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
        // رقم الفئة لهاد المنتج
        public int CategoryId { get; set; }
        // رقم العلامة التجارية لهاد المنتج
        public int BrandId { get; set; }
    }
}
