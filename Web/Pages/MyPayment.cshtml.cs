using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Pages
{
    public class MyPaymentModel : PageModel
    {
        private readonly Web.DbConnection.WebContext _context;

        public MyPaymentModel(Web.DbConnection.WebContext context)
        {
            _context = context;
        }

        public IList<Payment> Payment { get;set; } = default!;

		public async Task OnGetAsync()
		{
			if (_context.Payments != null)
			{
				// Get the current user's ID
				var userId = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name)?.UserId;

				// Filter the payments to only include those for the current user and order them by the most recent
				Payment = await _context.Payments
					.Where(p => p.UserId == userId)
					.OrderByDescending(p => p.PaymentDate) // This will sort the payments by date, most recent first
					.Include(p => p.User).ToListAsync();
			}
		}


	}
}
