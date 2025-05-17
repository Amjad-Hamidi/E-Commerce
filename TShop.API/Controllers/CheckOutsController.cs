using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Security.Claims;
using TShop.API.Services;
using TShop.API.Models;
using TShop.API.DTOs.Orders.Requests;
using TShop.API.Services.Orders;
using TShop.API.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TShop.API.Services.OrderItems;

namespace TShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // بس يجي المشتري بجبله اياهن من السلة مش هو برجع يختار ليشتري, بالتالي لازم يكون مسجل دخول
    public class CheckOutsController : ControllerBase
    {
        private readonly ICartService cartService;
        private readonly IOrderService orderService;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrderItemService orderItemService;

        public CheckOutsController(ICartService cartService, IOrderService orderService,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IOrderItemService orderItemService)
        {
            this.cartService = cartService;
            this.orderService = orderService;
            this.emailSender = emailSender;
            this.userManager = userManager;
            this.orderItemService = orderItemService;
        }

        [HttpGet("Pay")] // stripe تاعت ال DB هسا داخليا اه ضاف داتا على URL لانه بالاخر برجعلي رابط Get
        public async Task<IActionResult> Pay([FromBody] PaymentRequest paymentRequest)
        {
            var appUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // تبع اليوزر Cart اجيب ال
            var carts = await cartService.GetAsync(e => e.ApplicationUserId == appUser, [e => e.Product]); // include

            if (carts is null) // cart is empty
                return NotFound("No Cart Found");


            Order order = new() // بس عشان قدام رح افضيها فبضل محفوظ cart مع اني حسبته لل order لل price رح احسب ال
            {
                OrderStatus = OrderStatus.Pending,
                OrderDate = DateTime.Now,
                TotalPrice = carts.Sum(e => e.Product.Price * e.Couunt), // سعر المنتج * عدد تكراره في السلة
                ApplicationUserId = appUser
            };
            if (paymentRequest.PaymentMethod == PaymentMethodType.Cash.ToString())
            {
                order.PaymentMethodType = PaymentMethodType.Cash;
                await orderService.AddAsync(order);
                return RedirectToAction(nameof(Success), new { orderId = order.Id });
            }
            else if (paymentRequest.PaymentMethod == PaymentMethodType.Visa.ToString())
            {
                await orderService.AddAsync(order); // فلازم يتعرف عليه orderId لازم تكون بالبادية عشان رح نستعمل تحت

                order.PaymentMethodType = PaymentMethodType.Visa;
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" }, // الصفحة الخاصة بالدفع
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment", // refund ممكن يكون مود ال
                    SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Success/{order.Id}",
                    CancelUrl = $"{Request.Scheme}://{Request.Host}/api/CheckOuts/Cancel"
                };

                foreach (var item in carts)
                {
                    options.LineItems.Add(
                         new SessionLineItemOptions   // StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];  Program.cs ما بلزم لانو موجود في ال 

                         {
                             PriceData = new SessionLineItemPriceDataOptions
                             {
                                 Currency = "USD", // dollar, من خلال اني بعرف من وين هو وبناء عليه بحط شو العملة Globalization في اشي ثاني اسمه
                                 ProductData = new SessionLineItemPriceDataProductDataOptions
                                 {
                                     Name = item.Product.Name,
                                     Description = item.Product.Description,
                                 },
                                 UnitAmount = (long)item.Product.Price * 100, // عشان الفواصل
                             },
                             Quantity = item.Couunt, // item(product) كم اليوزر طالب عدد من هاد ال
                         });
                }

                var service = new SessionService();
                var session = service.Create(options);
                order.SessionId = session.Id;
                await orderService.CommitAsync();

                return Ok(new { session.Url }); // تبع الدفع URL برجع له ال 
            }
            else
            {
                return BadRequest(new { message = "invalid payment" });
            }

        }

        [HttpGet("Success/{orderId}")]
        [AllowAnonymous] // عشان ننهي قصة التوكن انو لازم يكون معه لتفتح
        public async Task<IActionResult> Success([FromRoute] int orderId)
        {
            var order = await orderService.GetOneAsync(e => e.Id == orderId);

            var applicationUser = await userManager.FindByIdAsync(order.ApplicationUserId);
            var subject = "";
            var body = "";

            var carts = await cartService.GetAsync(e => e.ApplicationUserId == applicationUser.Id,
                [e => e.Product]); // وافضي السلة OrderItems عشان هسا انقلهن لل products لل include

            List<OrderItem> orderItems = [];
            foreach(var item in carts)
            {
                orderItems.Add(new() // before : await orderItemService.AddAsync (more time = more requests)
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    TotalPrice = item.Couunt * order.TotalPrice
                });

                item.Product.Quantity -= item.Couunt;
            }

            await orderItemService.AddRangeAsync(orderItems);

            await cartService.RemoveRangeAsync(carts.ToList());

            await orderService.CommitAsync();


            if(order.PaymentMethodType == PaymentMethodType.Cash)
            {
                subject = "Order Recieved - Cash Payment";
                body = $"<h1>Hello {applicationUser.UserName}</h1>"
                    + $"<p>your order with {orderId} has been placed successfully.</p>";
              
            }
            else
            {
                order.OrderStatus = OrderStatus.Approved;
                var service = new SessionService();
                var session = service.Get(order.SessionId);

                order.TransactionId = session.PaymentIntentId;

                await orderService.CommitAsync();

                subject = "Order Payment Success";
                body = $"<h1>Hello {applicationUser.UserName}</h1>"
                   + $"<p>thank you for shopping with t-shop10.</p>";
            }

            await emailSender.SendEmailAsync(applicationUser.Email, subject, // OR => _ = emailSender.SendEmailAsync
                body);


            // هون بدي اعمل عملية التحديث على السلة
            return Ok(new { message = "Done" });
        }


    }
}