using Microsoft.EntityFrameworkCore;

namespace TShop.API.Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class OrderItem
    {
        // Order
        public int OrderId { get; set; }
        public Order Order { get; set; }
        // Product
        public int ProductId { get; set; }
        public Product Product { get; set; }


        public decimal TotalPrice { get; set; } // orders لجميع ال
        public string? Note { get; set; } // رسالة اختيارية
    }
}
