using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TShop.API.DTOs.Logins;
using TShop.API.DTOs.Registers;
using TShop.API.Models;
using TShop.API.Services.Passwords;
using TShop.API.Utility;

namespace TShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IPasswordResetCodeService passwordResetCodeService;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender, // والايميل gmail موضوع ال 
            RoleManager<IdentityRole> roleManager,
            IPasswordResetCodeService passwordResetCodeService) // Roles هو الي بتحكم بموضوع ال
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.roleManager = roleManager;
            this.passwordResetCodeService = passwordResetCodeService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] TShop.API.DTOs.Registers.RegisterRequest registerRequest)
        {
            var applicationUser = registerRequest.Adapt<ApplicationUser>();
            var result = await userManager.CreateAsync(applicationUser, registerRequest.Password);
        
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(applicationUser, StaticData.Customer); // Customer اله Role تلقائيا اي واحد بسجل بتكون ال            

                var token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser); // انشاء توكن صالحة للاستخدام مرة واحدة فقط
                var emailConfirmUrl = Url.Action(nameof(ConfirmEmail), // ConfirmEmail Action بوديه على ال 
                    "Account", //  Controller اسم ال
                    new {  token , userId = applicationUser.Id },
                    protocol: Request.Scheme, // http or https
                    host: Request.Host.Value // localhost: 7220
                    ); // انشاء رابط التفعيل
                       // هيك الشكل بكون https://localhost:7220/api/Account/ConfirmEmail?token=CfDJ8PRWtcYo9hZNjyWHkS9OPy14Py31ZkQaLGJWk%2B4fD9BwVltR%2BtAnc%2BfYik6NbrU3CsCNIYFt%2BccjKPqBdKYQndrGNz5EoLXf6sK5KjqwIVrVo5tYgJgoFNVpdNJffvmPF4I4uE1MY4yMqbZum2S%2FvBoM4PajVWQzso9MGpzmaYszKddrk6dGPZqCKaZeUEsmIL%2FkTK%2FjU3lkju%2BtSQO0ysKmdgzmwXGzMBbHE7HaU2HbXhp2jmjXmZ6moqUEyzRgzQ%3D%3D&userId=72df3f64-e233-490f-9697-f270dfe6f008

                await emailSender.SendEmailAsync(applicationUser.Email, "Confirm Email", // OR => _ = emailSender.SendEmailAsync
                    $"<h1>Hello ... {applicationUser.UserName} </h1> <p> t-shop10 ... new account </p>"+
                    $"<a href='{emailConfirmUrl}'> click here </a>");
                
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Email confirmed successfully" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return NotFound();


            /* // OR :
            if (userId == null || token == null) return BadRequest();
            var applicationUser = await userManager.FindByIdAsync(userId);
            if (applicationUser == null) return NotFound();
            var result = await userManager.ConfirmEmailAsync(applicationUser, token);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Email confirmed successfully" });
            }
            return BadRequest(result.Errors);
            */
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TShop.API.DTOs.Logins.LoginRequest loginRequest)
        {
            ApplicationUser? applicationUser = await userManager.FindByEmailAsync(loginRequest.Email);
            if(applicationUser != null)
            {
                //bool result = await userManager.CheckPasswordAsync(applicationUser, loginRequest.Password);

                // 2FA زي تاكيد الايميل والباسورد نفسه وكمان اذا معموله بلوك او لا وبتفحص ال CheckPasswordAsync بتعمل شغلات زيادة عن ال PasswordSignInAsync 
                var result = await signInManager.PasswordSignInAsync(applicationUser,
                    loginRequest.Password,
                    loginRequest.RememberMe,
                    lockoutOnFailure: false); // lockoutOnFailure: false يعني ما رح يقفل الحساب بعد 5 محاولات فاشلة

                // JWT الي بدي احفظه في Payload (Claims) عبارة عن ال
                // JWT هاد هو ال claims انشاء ال
                List<Claim> claims = new();
                claims.Add(new(ClaimTypes.Name, applicationUser.UserName)); // Controller بسميه الاسم الي بدي اياه , يا بستعمل التاعهم جاهز يا انا الي بدي اياه , بس الي بختلف طريةق الاستدعاء في ال 
                // Login في ال id عشان يعرف يتعرف على ال "id" منحط وليس ال NameIdentifier هاي ضرورية ال 
                claims.Add(new Claim(ClaimTypes.NameIdentifier, applicationUser.Id)); // OR : claims.Add(new("id", applicationUser.Id));

                var userRoles = await userManager.GetRolesAsync(applicationUser);

                if(userRoles.Count > 0) // OR : userRoles != null
                {
                    foreach(var item in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item)); // OR : claims.Add(new Claim(ClaimTypes.Role, item)); controller الفرق في طريقة الاستدعاء في ال 
                    }
                }

                if (result.Succeeded)
                {
                    SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("iv6eD3QDOjmR4Emc8h247zX5BLMN2SnP"));

                    SigningCredentials signingCredentials = new SigningCredentials(
                        symmetricSecurityKey,
                        SecurityAlgorithms.HmacSha256 
                        );

                    var jwtToken = new JwtSecurityToken(
                        claims: claims, // Payload عبارة عن ال 
                        expires: DateTime.UtcNow.AddMinutes(30),  // iat هاد هو ال 
                        signingCredentials: signingCredentials
                        );

                    string token = new JwtSecurityTokenHandler().WriteToken(jwtToken); // JWT هاد هو ال

                    /*
                    // cookies انشاء ال 
                    await signInManager.SignInAsync(applicationUser, loginRequest.RememberMe); // منحدد يضل يفوت عسحابه بدون تسجيل دخول او لا RememberMe بناء على 
                   */

                    return Ok(new { token });
                }

                else
                {
                    if (result.IsLockedOut)
                    {
                        return BadRequest(new { Message = "Your account is locked out, please try again later." });
                    }
                    else if (result.IsNotAllowed) // Login عشان يقدر يعمل ConfirmEmail اذا ايميله مش ماكد لازم يعمل
                    {
                        return BadRequest(new { Message = "Email not confirmed, please confirm your email before logging." });
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        return BadRequest(new { Message = "Two factor authentication is required." });
                    }
                    else
                    {
                        return BadRequest(new { Message = "Invalid email or password" });
                    }
                        //return BadRequest(result); //  Succeeded,IsLockedOut,IsNotAllowed,RequiresTwoFactor  تحتوي على  result
                }

            }

            return BadRequest(new { Message = "Invalid email or password" });

        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // remove the cookies
            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {   // وهي نادرة الحصول cookies بس عندو DB واحنا منستخدمها عشان في حالة ما كان موجود في ال DB بتجيب معلوماته من ال : GetUserAsync(User)
            ApplicationUser? applicationUser = await userManager.GetUserAsync(User); 

            if (applicationUser != null)
            {
               var result = await userManager.ChangePasswordAsync(
                   applicationUser,
                   changePasswordRequest.OldPassword,
                   changePasswordRequest.NewPassword);
               
                if(result.Succeeded)
                {
                    return NoContent();
                }
                return BadRequest(result.Errors);
            }

            return BadRequest(new { Message = "User not found" });
        }


        [HttpPost("forgot-password")] // عالايميل Code ارسال ال
        public async Task<IActionResult> ForgotPassword(ForgetPasswordRequest forgetPasswordRequest)
        {
            var applicationUser = await userManager.FindByEmailAsync(forgetPasswordRequest.Email);
            if (applicationUser is not null)
            {
                var code = new Random().Next(1000,9999).ToString();

                await passwordResetCodeService.AddAsync(new PasswordResetCode // PasswordResetCodes على جدول ال Code اضافة ال
                {
                    ApplicationUserId = applicationUser.Id,
                    Code = code,
                    ExpirationCode = DateTime.Now.AddMinutes(30),
                });

                await emailSender.SendEmailAsync(applicationUser.Email, "Reset Password", 
                   $"<h1>Hello ... {applicationUser.UserName} </h1> <p> t-shop10 ... Reset Password </p>" +
                   $"Code is {code}");

                return Ok(new {message = "Reset Code Sent to your Email."});
            }
            else
                return BadRequest(new { message = "Email not found" });
        }


        [HttpPatch("send-code")] // وتحديث الباسورد DB ومقارنته بال Code ادخال ال
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest sendCodeRequest)
        {
            var appUser = await userManager.FindByEmailAsync(sendCodeRequest.Email);

            if (appUser is not null)
            {
                var resetCode = (await passwordResetCodeService.GetAsync(e => e.ApplicationUserId == appUser.Id))
                    .OrderByDescending(e => e.ExpirationCode).FirstOrDefault(); // PasswordResetCode احدث صف من , ExpirationCode بجيب احدث

                if (resetCode != null && resetCode.Code == sendCodeRequest.Code && resetCode.ExpirationCode > DateTime.Now)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(appUser);
                    var result = await userManager.ResetPasswordAsync(appUser, token, sendCodeRequest.Password);
                    
                    if(result.Succeeded)
                    {
                        await emailSender.SendEmailAsync(appUser.Email, "Password changed",
                          $"<h1>Hello ... {appUser.UserName} </h1> <p> t-shop10 ... your password is changed </p>");
                        // (PasswordResetCodes) جدول ال DB بعد ما يرسله عالايميل بحذفه من ال , DB من ال Code هيك بنحذف ال 
                        await passwordResetCodeService.RemoveAsync(resetCode.Id, CancellationToken.None);

                        return Ok(new {message = "Password has been changed successfully."});
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }

                }
                else
                {
                    return BadRequest(new { message = "Invalid Code." });

                }

            }

            return BadRequest(new { message = "User not found." });
        }
    }
}
