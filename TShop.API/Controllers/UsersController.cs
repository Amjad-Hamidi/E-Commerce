using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TShop.API.DTOs.Users;
using TShop.API.Services.Users;
using TShop.API.Utility;

namespace TShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{StaticData.SuperAdmin}")] // Cookies(default from identity) بتتعامل مع ال Authorize لانو ال Postman ملاحظة: ما منقدر حاليا نشتغله ع // Super Admin خاص بال 
    public class UsersController : ControllerBase 
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService) // Users هي الي بتجيب ال
        {
            this.userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var users = await userService.GetAsync();

            return Ok(users.Adapt<IEnumerable<UserDto>>());
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await userService.GetOneAsync(u => u.Id == id);

            if (user is null)
                return NotFound();

            return Ok(user.Adapt<UserDto>());
        }

        [HttpPost("Change Role {userId}")]
        public async Task<IActionResult> ChangeRole([FromRoute] string userId, [FromQuery] string newRoleName)
        {
            var result = await userService.ChangeRole(userId, newRoleName);

            return Ok(result);
        }

        [HttpPatch("LockUnLock/{userId}")]
        public async Task<IActionResult> LockUnLock([FromRoute] string userId)
        {
            var result = await userService.LockUnLock(userId);
            if(result == true)
                return Ok("User is Locked");
            else if (result == false)
                return Ok("User is UnLocked");
            else
                return NotFound("User Not Found");
        }


    }
}
