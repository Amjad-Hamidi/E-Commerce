using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.Passwords
{
    public class PasswordResetCodeService : Service<PasswordResetCode>, IPasswordResetCodeService
    {
        public PasswordResetCodeService(ApplicationDbContext context) : base(context)
        {
        }
    }
}
