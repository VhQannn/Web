using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.DbConnection;
using Web.DTOs;

namespace Web.Pages
{
    [Authorize]   
    
    public class ChangePasswordModel : PageModel
    {
        private WebContext _context;

        [BindProperty]
        public ChangePasswordConfirmDTO ChangePassword { get; set; }
        public string ErrorMessage { get; set; }
        public ChangePasswordModel(WebContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            ChangePassword = new ChangePasswordConfirmDTO();
          
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
           
            return Page();
        }
    }
}
