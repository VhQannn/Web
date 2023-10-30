using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.DbConnection;
using Web.IRepository;

namespace Web.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string username { get; set; }
        [BindProperty]
        public string password { get; set; }

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
                username = HttpContext.Request.Cookies["username"];
                password = HttpContext.Request.Cookies["password"];
            }
        }
        public void OnPost()
        {
            try
            {
                User currentUser = _userRepository.Login(username, password);
                int userId = currentUser.UserId;
                if (currentUser == null)
                {
                    TempData["Fail"] = "Đăng nhập thất bại!.";
                    Redirect("/Login");
                }
                else
                {
                    if (IsRemember)
                    {
                        // Lưu username vào cookie
                        HttpContext.Response.Cookies.Append("username", username, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(14) });

                        // Lưu một token vào cookie (ví dụ: bạn có thể lưu mật khẩu đã mã hóa)
                        HttpContext.Response.Cookies.Append("password", password, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(14) });
                    }
                    else
                    {
                        // Xóa cookies nếu người dùng không chọn "Nhớ mật khẩu"
                        HttpContext.Response.Cookies.Delete("username");
                        HttpContext.Response.Cookies.Delete("password");
                    }
                    HttpContext.Session.SetInt32("userId", userId);
                    HttpContext.Session.SetString("role", currentUser.UserType);
                    Response.Redirect("/Index");
                }
            }
            catch (Exception)
            {
                TempData["Fail"] = "Đăng nhập thất bại!.";
                Redirect("/Login");
            }

        }
    }
}
