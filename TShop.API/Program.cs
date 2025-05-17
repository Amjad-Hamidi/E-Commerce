using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Stripe;
using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services;
using TShop.API.Services.Brands;
using TShop.API.Services.Categories;
using TShop.API.Services.OrderItems;
using TShop.API.Services.Orders;
using TShop.API.Services.Passwords;
using TShop.API.Services.Products;
using TShop.API.Services.Users;
using TShop.API.Utility;
using TShop.API.Utility.DBInitializer;

namespace TShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            /*
            // ÇĞÇ ÈÏí ÇÚÏá ÚáíåÇ
            builder.Services.AddAuthorization();
            */


            // Swagger ÎÇÕíä İí ÇäÔÇÁ Çá Services åÏæá
            /*
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            */

            // IBrandsService ÚãáäÇ ÍŞä İíå áá BrandsController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<IBrandService, BrandService>();

            // IProductService ÚãáäÇ ÍŞä İíå áá ProductsController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<IProductService, Services.Products.ProductService>(); // Stripe.ProductService ãÔ

            // ICategoryService ÚãáäÇ ÍŞä İíå áá CategoriesController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            // ICartService ÚãáäÇ ÍŞä İíå áá CartController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<ICartService, CartService>();

            // IOrderService ÚãáäÇ ÍŞä İíå áá CheckOutsController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<IOrderService, OrderService>();

            // IPasswordResetCodeService ÚãáäÇ ÍŞä İíå áá AccountController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<IPasswordResetCodeService, PasswordResetCodeService>();

            // IOrderItemService ÚãáäÇ ÍŞä İíå áá CheckoutsController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();

            // Configure Stripe settings (ÈÌíÈ ÇáÈíÇäÇÊ ãä Çá  appsettings.json)
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            // DI (Dependency Injection) ãä ÎáÇá ØÑíŞÉ Çá DB ÇäÔÇÁ ÎÏãÉ ÇáÇÊÕÇá ÈÇá
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

            // SignInManager, UserManager, RoleManager, ApplicationUser, IdentityRole ÇäÔÇÁ ÎÏãÉ Çá
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>( options =>
                {
                    options.User.RequireUniqueEmail = false;
                    options.SignIn.RequireConfirmedEmail = true; // ConfirmedEmail ÛíÑ ãÇ íßæä ÚÇãá Login ããäæÚ ÍÏÇ íÚãá 
                })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // gmail ÈæÇÓØÉ IdentityServer ÇäÔÇÁ ÎÏãÉ Çá
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Çáí ÇáåÇ Service İáÇÒã äÓÌá Çá run Çæá ãÇ ÇÚãá seeding data Úãá 
            builder.Services.AddScoped<IDBInitializer, DBInitializer>();

            // IUserService ÚãáäÇ ÍŞä İíå áá UsersController İí Çá DI áÇäÇ ÚãáäÇ Service áá life cycle time íÌÈ ÊÍÏíÏ Çá
            builder.Services.AddScoped<IUserService, UserService>();

            // ÇÚÏá ÚáíåÇ jwt ÇäÇ ÖİÊåÇ ÚÔÇä ãæÖæÚ Çá 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // jwt Çáì cookies æåæ Çá identity áÇáÛÇÁ ÇáÇÔí ÇáÇİÊÑÇÖí áá 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // controller İí Çá [Authorize] İí Çá Not Authorized 401 Çáì Not Found 404 ÊÛííÑ Çá 
            }).AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false, // ÇáÊÍŞŞ ãä ãÕÏÑ ÇáÊæßä
                        ValidateAudience = false, // ÇáÊÍŞŞ ãä ÇáÌåÉ ÇáãÓÊİíÏÉ ãä ÇáÊæßä
                        ValidateLifetime = true, // ÇáÊÍŞŞ ãä ÕáÇÍíÉ ÇáÊæßä ÈÇáæŞÊ ÊÇÚåÇ ÇäÊåì æáÇ áÇ
                        ValidateIssuerSigningKey = true, // secret key ÇáÊÃßÏ ãä ÇáÊæßä ãÔ ãÒæÑ Çí Çäå 
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("iv6eD3QDOjmR4Emc8h247zX5BLMN2SnP")),
                    };
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
                /*
                // swagger ÎÇÕíä ÈÊßæíä Çá 
                app.UseSwagger();
                app.UseSwaggerUI();
                */
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            // ÈÏí ÇÛíÑ ÇáÓáæß ÇáÇİÊÑÇÖí UseAuthorization ÇäÇ ÖİÊåÇ , áÇÒã Êßæä ŞÈá Çá
            app.UseAuthorization();

            // CORS áÇãßÇäíÉ ÇáÑÈØ ãÚ ÇáİÑæäÊ
            // var builder2 = WebApplication.CreateBuilder(args);

            // CORS áÇãßÇäíÉ ÇáÑÈØ ãÚ ÇáİÑæäÊ
            app.UseCors(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());


            /* ÈäÌÇÍ Çã áÇ DB İŞØ ÊÍŞŞ åá Êã ÇáÇÊÕÇá ÈÇá , DI ŞÈá ãÇ äÚãá
            var context = new ApplicationDbContext();
            try
            {
                context.Database.CanConnect();
                Console.WriteLine("done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
            }
            */

            app.MapControllers();

            // (Super Admin áá seeding data åÇÏ ÇáßæÏ ÑÍ íÊÔÛá ãÑÉ æÍÏÉ İŞØ (ãæÖæÚ Çá
            // IDBInitializer æåæ ãÑÈæØ ãÚ Çá DB áá seeding ÈäÚãá 
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitializer>();
            service.initialize();

            app.Run();
        }
    }
}
