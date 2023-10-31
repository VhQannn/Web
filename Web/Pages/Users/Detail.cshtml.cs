using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Pages.Users
{
    public class DetailModel : PageModel
    {
        public User User { get; set; } = default!;

        public DetailModel()
        {
        }


        public async Task<IActionResult> OnGetAsync()
        {

            return Page();
        }
    }
}
