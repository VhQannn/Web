using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.DbConnection;

namespace Web.Pages
{
    [Authorize(Roles = "Admin")]
    public class PaymentsModel : PageModel
    {
        private readonly WebContext _context;
        private const int PageSize = 20;

        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PaymentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        public PaymentsModel(WebContext context)
        {
            _context = context;
        }

        public List<Payment> Payments { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

        public async Task OnGetAsync(int currentPage = 1)
        {
            IQueryable<Payment> paymentQuery = _context.Payments.Include(p => p.User);

            if (!string.IsNullOrEmpty(Username))
            {
                paymentQuery = paymentQuery.Where(p => p.User.Username.Contains(Username));
            }

            if (PaymentId.HasValue)
            {
                paymentQuery = paymentQuery.Where(p => p.PaymentId == PaymentId.Value);
            }

            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                paymentQuery = paymentQuery.Where(p => p.Status == StatusFilter);
            }

            var totalPaymentsCount = await paymentQuery.CountAsync();
            TotalPages = (int)System.Math.Ceiling(totalPaymentsCount / (double)PageSize);

            Payments = await paymentQuery
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            CurrentPage = currentPage;
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int paymentId, string newStatus, string username, int? paymentIdFilter, string statusFilter)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                payment.Status = newStatus;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { Username = username, PaymentId = paymentIdFilter, StatusFilter = statusFilter });
        }
    }
}
