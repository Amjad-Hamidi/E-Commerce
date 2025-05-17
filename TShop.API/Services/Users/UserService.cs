using Microsoft.AspNetCore.Identity;
using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services.IService;
using TShop.API.Services.Users;

namespace TShop.API.Services.Categories
{
    public class UserService : Service<ApplicationUser>, IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            this._context = context;
            _userManager = userManager;
        }


        public async Task<bool> ChangeRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                // remove the old role
                var oldRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRoles);

                // add the new role
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }


            return false;
        }


        public async Task<bool?> LockUnLock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            var isLockedNow = user.LockoutEnabled // LockoutEnabled = true معناها انو في حظر
                && user.LockoutEnd.HasValue // LockoutEnd.HasValue = true معناها انو في وقت حظر
                && user.LockoutEnd.Value > DateTimeOffset.UtcNow; // واكبر من الوقت الحالي  block اذا كان معموله 
            if(isLockedNow)
            {
                // unlock the user بنفك الحظر
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
            }
            else
            {
                // lock the user بحظر المستخدم
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(5); // 5 minutes             
            }

            var result = await _userManager.UpdateAsync(user);

            return !isLockedNow; // return the new status

        }

    }
}
