using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class ExternalLoginModel : PageModel
    {
        public IActionResult OnGet(string provider)
        {
            return new ChallengeResult(provider, new AuthenticationProperties { RedirectUri = "/externalresponse" });
        }
    }
}
