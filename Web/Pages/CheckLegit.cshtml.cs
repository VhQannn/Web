using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Pages
{
    public class CheckLegitModel : PageModel
    {
        private readonly WebContext _context;

        public CheckLegitModel(WebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        public bool? UserFound { get; set; }

        // Add a property to hold user details
        public User UserDetails { get; set; }

        public void OnPost()
        {
            var user = _context.Users
                .Include(u => u.UserSupporterInsurances) // Ensure related data is included
                .SingleOrDefault(u => u.Username == Username);

            if (user != null)
            {
                UserDetails = user; // Store user details

                var currentDate = DateTime.Now;
                var insurance = user.UserSupporterInsurances
                    .FirstOrDefault(i => i.StartDate <= currentDate && i.EndDate >= currentDate);

                UserFound = insurance != null;
            }
            else
            {
                UserFound = null;
            }
        }
    }
}
