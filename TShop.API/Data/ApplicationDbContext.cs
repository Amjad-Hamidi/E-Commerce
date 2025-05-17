using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TShop.API.Models;

namespace TShop.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor
        // بيستخدم ال DbContextOptions عشان يحدد نوع ال DB الي هيتصل بيه
        // وبيستخدم ال optionsBuilder عشان يحدد ال connection string
        // وبيستخدم ال base عشان ينادي علي ال constructor بتاع ال DbContext
        // DB المسؤول عن الاتصال في OnConfiguring الي اصلا بتحتوي جواتها عفنكشن ال DbContext هي اصلا ال base
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Call the base method to apply default configurations

            builder.Entity<Cart>().HasKey(c => new { c.ProductId, c.ApplicationUserId }); // Composite Key in Cart Table
        }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Database=tshop10;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
        */

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; } 
        public DbSet<Brand> Brands { get; set; }

        public DbSet<Cart> Carts { get; set; } 

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

    }
}
