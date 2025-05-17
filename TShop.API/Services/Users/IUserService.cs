using System.Linq.Expressions;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.Users
{
    public interface IUserService : IService<ApplicationUser> // Generic Interface
    {
        Task<bool> ChangeRole(string userId, string roleName);
        Task<bool?> LockUnLock(string userId);
    }
}
