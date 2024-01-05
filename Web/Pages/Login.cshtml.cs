using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Web.DbConnection;
using Web.IRepository;

namespace Web.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập username.")]
        public string Username { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 kí tự.")]
        public string Password { get; set; }

        [BindProperty]
        public bool IsRemember { get; set; }
        private IUserRepository _userRepository;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public LoginModel(IUserRepository userRepository, IAuthenticationSchemeProvider schemeProvider)
        {
            _userRepository = userRepository;
            _schemeProvider = schemeProvider;
        }
        public async Task OnGet()
        {
            var allSchemes = await _schemeProvider.GetAllSchemesAsync();
            ExternalLogins = allSchemes.Where(s => !string.IsNullOrEmpty(s.DisplayName)).ToList();
        }
        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ViewData["Error"] = "Tên đăng nhập và mật khẩu là bắt buộc.";
                return Redirect("/login");
            }

            var currentUser = _userRepository.Login(Username, Password);
            if (currentUser == null)
            {
                ViewData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return Redirect("/login");
            }
            HttpContext.Session.SetInt32("UserId", currentUser.UserId);
            HttpContext.Session.SetString("UserName", currentUser.Username);
            HttpContext.Session.SetString("Role", currentUser.UserType.ToString());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, currentUser.Username),
                new Claim(ClaimTypes.Role, currentUser.UserType.ToString()),
                new Claim(ClaimTypes.NameIdentifier, currentUser.UserId.ToString())
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
