using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using Web.DbConnection;
using Web.DTOs;

namespace Web.Pages
{
    public class ForgetPasswordModel : PageModel
    {
        [BindProperty]
        public VerifyPasswordDTO VerifyPassword { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        private readonly WebContext _context;
        private readonly IHttpClientFactory _clientFactory;
        public ForgetPasswordModel(WebContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        public void OnGet()
        {
            VerifyPassword = new VerifyPasswordDTO();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username.Equals(VerifyPassword.Username) && x.Email.Equals(VerifyPassword.Email));
            if (user == null)
            {
                ErrorMessage = "Không tìm thấy người dùng với thông tin đã cung cấp.";
                return Page();
            }

            // Gọi API
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsync("https://localhost:7247/api/email/forget-password", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Có lỗi xảy ra khi gửi yêu cầu đến API.";
            }
            else
            {
                SuccessMessage = "Đã gửi mật khẩu mới vào email. Vui lòng kiểm tra email của bạn";
            }
            return Page();

        }
    }
}
