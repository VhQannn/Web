using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PostManagementModel : PageModel
    {

        public void OnGet()
        {
        }
    }
}
