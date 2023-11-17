using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UploadQuestionModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
