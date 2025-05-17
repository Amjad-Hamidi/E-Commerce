using Microsoft.EntityFrameworkCore;

namespace TShop.API.Models
{
    // [PrimaryKey(nameof(ProductId), nameof(ApplicationUserId))] // Composite Key in Cart Table ( ApplicationDbContext معمولة ايضا في ال)
    public class Cart
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // warning بس عشان اشيل ال
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!; // warning بس عشان اشيل ال

        public int Couunt { get; set; } 
    }
}
