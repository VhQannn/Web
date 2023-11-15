using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.DTOs;

namespace Web.Pages
{
    [Authorize]
    public class CheckResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Uri { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public void OnGet()
        {
        }
    }
}
