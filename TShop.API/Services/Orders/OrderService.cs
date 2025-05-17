using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.Orders
{
    public class OrderService : Service<Order>, IOrderService
    {
        public OrderService(ApplicationDbContext context) : base(context)
        {
        }


    }
}
