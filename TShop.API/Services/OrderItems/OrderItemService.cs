using Microsoft.VisualBasic;
using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.OrderItems
{
    public class OrderItemService : Service<OrderItem>, IOrderItemService
    {
        private readonly ApplicationDbContext context;

        public OrderItemService(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }


        public async Task<List<OrderItem>> AddRangeAsync(List<OrderItem> orderItems, CancellationToken cancellationToken = default)
        {
            await context.AddRangeAsync(orderItems, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return orderItems;
        }



    }
}
