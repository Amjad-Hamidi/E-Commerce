using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services
{
    public class CartService : Service<Cart>, ICartService 
    {
        private readonly ApplicationDbContext _context;

        // ليمررها معه base لذلك نستخدم هون ال default مش رح يكون موجود ال parameterized constructor عشان موجود ال Service<Category> هون مهمة كثير عشان لما يروح على base
        public CartService(ApplicationDbContext context) : base(context) // DbContext الي هو Service لل _context بمرر ال 
        {
            this._context = context;
        }

        public async Task<Cart> AddToCart(string UserId, int productId, CancellationToken cancellationToken)
        {
            var existingCartItem = await _context.Carts // Carts: (ApplicationUserId + ProductId) are P.K
                .FirstOrDefaultAsync(cart => cart.ApplicationUserId == UserId && cart.ProductId == productId);
            // Cart معناها هاد اليوزر في عندو c => c.ApplicationUserId == UserId 
            // موجود عندو من قبل Product معناها هاد اليوزر ال  c.ProductId == productId 
            if (existingCartItem != null) // معناها المنتج عندو موجود من قبل
            {
                existingCartItem.Couunt++;
            }
            else // معناها المنتج هاد جديد لليوزر , غير موجود مسبقا
            {
                existingCartItem = new Cart
                {
                    ProductId = productId,
                    ApplicationUserId = UserId,
                    Couunt = 1
                };
                // SaveChanges() داخليا بتعمل AddAsync()
                await _context.Carts.AddAsync(existingCartItem, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken); // وليس فقط تعديل بدون تخزين DB ضرورية لانو بدنا نخزن بال 
            return existingCartItem;
        }


        public async Task<IEnumerable<Cart>> GetUserCartAsync(string UserId)
        {
            return await GetAsync(cart => cart.ApplicationUserId == UserId, includes: [cart => cart.Product] ); // نادى وبنفذه Cart من حاله بعرف انو ال 

            /* // OR :
            var cartItems = await _context.Carts
                .Include(c => c.Product) // Include the Product details
                .Where(c => c.ApplicationUserId == UserId)
                .ToListAsync(cancellationToken);
            return cartItems;
            */
        }

        public async Task<bool> RemoveRangeAsync(List<Cart> items, CancellationToken cancellationToken = default)
        {
            _context.Carts.RemoveRange(items); // error: _context.RemoveRange(items, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }




    }
}
