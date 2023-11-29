using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Web.DbConnection;
using Web.IRepository;
using Web.Services;

namespace Web.Pages
{
    public class ExternalResponseModel : PageModel
    {
        private WebContext _context;
        private IUserRepository _userRepository;
        private EmailService _emailService;

        public string Message { get; set; }

        public ExternalResponseModel(WebContext context, IUserRepository userRepository, EmailService emailService)
        {
            _context = context;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task SetLoginInforAsync(User user)
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("Role", user.UserType.ToString());

            var newClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                // Add user ID as a claim
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Request.Query.ContainsKey("error"))
            {
                return RedirectToPage("/Login", new { message = "External authentication was cancelled." });
            }
            if (User.Identity.IsAuthenticated)
            {
                // Lấy thông tin người dùng
                var claims = User.Claims.ToList();
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var check = _context.Users.SingleOrDefault(x => x.Email.Equals(email));
                if (check != null)
                {
                    if (check.IsVerify == false)
                    {
                        return Redirect("/login");
                    }

                    await SetLoginInforAsync(check);
                    return RedirectToPage("/Index");

                }
                else
                {
                    string username = email.Substring(0, email.IndexOf("@"));
                    string password = _userRepository.RandomPassword();
                    var registerCheck = _userRepository.Register(new Models.RegisterDTO
                    {
                        Email = email,
                        Username = username,
                        Password = password,
                        ConfirmPassword = password
                    }, true);
                    await _emailService.SendEmailRandomPassword(email, password);
                    if (registerCheck)
                    {
                        Message = "Đã đăng kí tài khoản thành công. Mật khẩu đã được gửi vào email của bạn";
                        var user = _context.Users.SingleOrDefault(x => x.Email.Equals(email));
                        await SetLoginInforAsync(user);
                        return Redirect("/");
                    }
                    else
                    {
                        Message = "Có lỗi xảy ra";
                        return Page();
                    }
                    
                }
            }
            return Page();
        }
    }
}
