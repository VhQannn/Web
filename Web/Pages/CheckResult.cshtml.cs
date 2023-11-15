using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class CheckResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Uri { get; set; }

        public void OnGet()
        {
        }
    }
}
