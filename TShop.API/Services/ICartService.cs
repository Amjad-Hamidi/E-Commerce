using Microsoft.AspNetCore.Mvc;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services
{
    public interface ICartService : IService<Cart>
    {
        Task<Cart> AddToCart(string UserId, int productId, CancellationToken cancellationToken);
        Task<IEnumerable<Cart>> GetUserCartAsync(string UserId);
        Task<bool> RemoveRangeAsync(List<Cart> items, CancellationToken cancellationToken = default);
    }
}
