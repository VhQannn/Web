using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class GoogleLoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new ChallengeResult(
                 GoogleDefaults.AuthenticationScheme,
                 new AuthenticationProperties
                 {
                     RedirectUri = "/GoogleResponse"
                 });
        }
    }
}
