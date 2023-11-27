using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.DTOs;
using Web.IRepository;

namespace Web.Pages
{
    public class AccountSettingModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int uId { get; set; }

        public int accountId { get; set; }
        [BindProperty]
        public ChangePasswordConfirmDTO ChangePassword { get; set; }

        private IUserRepository _userRepository;
        private WebContext _context;

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public AccountSettingModel(WebContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
            accountId = uId;
        }

        public void OnGet()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            ChangePassword = new ChangePasswordConfirmDTO();
        }

        public async Task<ActionResult> OnPostChangePass()
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserId == uId);
            var checkOldPass = _userRepository.Login(user.Username, ChangePassword.OldPassword);
            if(checkOldPass == null)
            {
                TempData["ErrorMessage"] = "Sai mật khẩu";
                return Redirect($"/accountsetting?uId={uId}&tab=changePassword");
            }
            var check = await _userRepository.ChangePassword(user.Username, ChangePassword.NewPassword);
            if (check == null)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra";
                return Redirect($"/accountsetting?uId={uId}&tab=changePassword");
            }

            TempData["SuccessMessage"] = "Đã đổi mật khẩu thành công";
            return Redirect($"/accountsetting?uId={uId}&tab=changePassword");

        }
    }
}
