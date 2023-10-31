using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.DbConnection;
using Web.IRepository;
using Web.Models;

namespace Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public RegisterModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public RegisterDTO User { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || User == null)
            {
                return Page();
            }

            bool isSuccess = _userRepository.Register(User);
            if(!isSuccess)
            {
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
