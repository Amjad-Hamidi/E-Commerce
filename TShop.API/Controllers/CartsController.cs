using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TShop.API.DTOs.Carts;
using TShop.API.Models;
using TShop.API.Services;

namespace TShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // الي اله , المهم انو مسجل دخول Role بغض النظر شو ال
    public class CartsController : ControllerBase
    {
        private readonly ICartService cartService;
        private readonly UserManager<ApplicationUser> userManager;

        public CartsController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            this.cartService = cartService;
            this.userManager = userManager;
        }

        
        [HttpPost("{ProductId}")] // بدونها ممكن اضيف وهو مش موجود Cart لجدول ال F.K لانو هي عبارة عن error 500 ضرورية جدا ولا بجيب 
        public async Task<IActionResult> AddToCart([FromRoute] int ProductId, CancellationToken cancellationToken) // من فوق authorize لانو بيجي ApplicationUserId عشان احدد كم العدد , ما في داعي لل count ال
        {
            var appUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // اذا وصل هون , لذلك لا داعي لفحصها not null اكيد رح يكون 
            var result = await cartService.AddToCart(appUser, ProductId, cancellationToken);
            return Ok();


            /*
            // key(NameIdentifier) الي مخزنها في ال value بتجيب ال 
            var appUser2 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Login في ال NameIdentifier لانا عملناها , من ال jwt الان بصير يجيبها بناء على ال

            // OR :
            string? appUser = userManager.GetUserId(User); // Login في ال NameIdentifier لانا عملناها , من ال jwt الان بصير يجيبها بناء على ال
            var cart = new Cart()
            {
                ProductId = ProductId,
                ApplicationUserId = appUser,
                Couunt = 1
            };

            await cartService.AddAsync(cart); // cart بضيفه على ال 

            return Ok(cart);
            */
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUserCartAsync()
        {
            var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var cartItems = await cartService.GetUserCartAsync(appUser); // return => IEnumerable<Cart>

            // Adapt<> ثم بعمل Select لذلك بستخدم cartItems فقط الي بكون داخل ال Product obj انا بدي اوصل لل 
            var cartResponse = cartItems.Select(e => e.Product).Adapt<IEnumerable<CartResponse>>();

            // الي هو عدد تكرار هاد المنتج Cart هون هي الي مخزنة جوا ال Count لاحظ ال 
            var totalPrice = cartItems.Sum(e => e.Product.Price * e.Couunt); // cartItems.Count() * cartItems.Price

            return Ok(new { cartResponse, totalPrice});
        }

    }
}
