using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Web.DbConnection;
using Web.IRepository;

namespace Web.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public bool IsRemember { get; set; }
        private IUserRepository _userRepository;

        public LoginModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void OnGet()
        {
            if (HttpContext.Request.Cookies.ContainsKey("username"))
            {
                Username = HttpContext.Request.Cookies["username"];
                Password = HttpContext.Request.Cookies["password"];
            }
        }
        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ViewData["Error"] = "Tên đăng nhập và mật khẩu là bắt buộc.";
                return Page();
            }

            var currentUser = _userRepository.Login(Username, Password);
            if (currentUser == null)
            {
                ViewData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return Page();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, currentUser.Username),
            new Claim(ClaimTypes.Role, currentUser.UserType.ToString())
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = IsRemember
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToPage("/Index");

        }
    }
}
