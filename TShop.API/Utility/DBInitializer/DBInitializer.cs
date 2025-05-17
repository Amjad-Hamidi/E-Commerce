using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TShop.API.Data;
using TShop.API.Models;

namespace TShop.API.Utility.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public DBInitializer(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        public async Task initialize()
        {
            try
            {
                if (context.Database.GetPendingMigrations().Any()) // Check if there are any pending migrations
                    context.Database.Migrate(); // Apply any pending migrations to the database (create the database if it doesn't exist) 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (roleManager.Roles is not null) // OR : roleManager.Roles.IsNullOrEmpty() // بالمرة ولا لا Roles بتفحص هل فيه 
            {
                await roleManager.CreateAsync(new(StaticData.SuperAdmin)); 
                await roleManager.CreateAsync(new(StaticData.Admin)); 
                await roleManager.CreateAsync(new(StaticData.Customer)); 
                await roleManager.CreateAsync(new(StaticData.Company)); 

                /* // OR :
                IdentityRole role = new IdentityRole("SuperAdmin");
                await roleManager.CreateAsync(role); // CreateAsync(IdentityRole role)
                await roleManager.CreateAsync(new("Admin")); // new without => new IdentityRole, this is syntax sugar
                await roleManager.CreateAsync(new("Customer"));
                await roleManager.CreateAsync(new("Company"));
                */

                await userManager.CreateAsync(new()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "super_admin",
                    Gender = ApplicationUserGender.Male,
                    BirthDate = new DateTime(2003, 10, 24),
                    Email = "admin@tshop10.com"
                }, "Admin@123");

                var user = await userManager.FindByEmailAsync("admin@tshop10.com");
                await userManager.AddToRoleAsync(user, StaticData.SuperAdmin); // add SuperAdmin role to the user (seed data)

                // OR :
                //await userManager.AddToRoleAsync(userManager.Users.FirstOrDefault(e => e.UserName == "super_admin"), "SuperAdmin");
            }



        }
    }
}
