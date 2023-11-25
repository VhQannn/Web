using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.DTOs;

namespace Web.Pages
{
    public class AccountSettingModel : PageModel
    {
		[BindProperty(SupportsGet = true)]
		public int uId { get; set; }

        [BindProperty]
        public ChangePasswordConfirmDTO ChangePassword { get; set; }

        public void OnGet()
        {
            ChangePassword = new ChangePasswordConfirmDTO();
        }
    }
}
