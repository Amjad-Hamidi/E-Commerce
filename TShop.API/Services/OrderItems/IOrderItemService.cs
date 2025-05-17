using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.OrderItems
{
    
    public interface IOrderItemService : IService<OrderItem>
    {
        Task<List<OrderItem>> AddRangeAsync(List<OrderItem> orderItems, CancellationToken cancellationToken = default);
    }
    
}
