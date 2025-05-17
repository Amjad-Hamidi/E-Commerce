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
            // ��� ��� ���� �����
            builder.Services.AddAuthorization();
            */


            // Swagger ����� �� ����� �� Services ����
            /*
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            */

            // IBrandsService ����� ��� ��� �� BrandsController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<IBrandService, BrandService>();

            // IProductService ����� ��� ��� �� ProductsController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<IProductService, Services.Products.ProductService>(); // Stripe.ProductService ��

            // ICategoryService ����� ��� ��� �� CategoriesController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            // ICartService ����� ��� ��� �� CartController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<ICartService, CartService>();

            // IOrderService ����� ��� ��� �� CheckOutsController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<IOrderService, OrderService>();

            // IPasswordResetCodeService ����� ��� ��� �� AccountController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<IPasswordResetCodeService, PasswordResetCodeService>();

            // IOrderItemService ����� ��� ��� �� CheckoutsController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();

            // Configure Stripe settings (���� �������� �� ��  appsettings.json)
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            // DI (Dependency Injection) �� ���� ����� �� DB ����� ���� ������� ���
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

            // SignInManager, UserManager, RoleManager, ApplicationUser, IdentityRole ����� ���� ��
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>( options =>
                {
                    options.User.RequireUniqueEmail = false;
                    options.SignIn.RequireConfirmedEmail = true; // ConfirmedEmail ��� �� ���� ���� Login ����� ��� ���� 
                })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // gmail ������ IdentityServer ����� ���� ��
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // ��� ���� Service ����� ���� �� run ��� �� ���� seeding data ��� 
            builder.Services.AddScoped<IDBInitializer, DBInitializer>();

            // IUserService ����� ��� ��� �� UsersController �� �� DI ���� ����� Service �� life cycle time ��� ����� ��
            builder.Services.AddScoped<IUserService, UserService>();

            // ���� ����� jwt ��� ����� ���� ����� �� 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // jwt ��� cookies ��� �� identity ������ ����� ��������� �� 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // controller �� �� [Authorize] �� �� Not Authorized 401 ��� Not Found 404 ����� �� 
            }).AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false, // ������ �� ���� ������
                        ValidateAudience = false, // ������ �� ����� ��������� �� ������
                        ValidateLifetime = true, // ������ �� ������ ������ ������ ����� ����� ��� ��
                        ValidateIssuerSigningKey = true, // secret key ������ �� ������ �� ���� �� ��� 
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
                // swagger ����� ������ �� 
                app.UseSwagger();
                app.UseSwaggerUI();
                */
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            // ��� ���� ������ ��������� UseAuthorization ��� ����� , ���� ���� ��� ��
            app.UseAuthorization();

            // CORS �������� ����� �� �������
            // var builder2 = WebApplication.CreateBuilder(args);

            // CORS �������� ����� �� �������
            app.UseCors(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());


            /* ����� �� �� DB ��� ���� �� �� ������� ��� , DI ��� �� ����
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

            // (Super Admin �� seeding data ��� ����� �� ����� ��� ���� ��� (����� ��
            // IDBInitializer ��� ����� �� �� DB �� seeding ����� 
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitializer>();
            service.initialize();

            app.Run();
        }
    }
}
