using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class WidthdrawalRequestManagementModel : PageModel
    {
        private readonly WebContext _context;
        private const int PageSize = 20;

        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RequestId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        public WidthdrawalRequestManagementModel(WebContext context)
        {
            _context = context;
        }

        public List<WithdrawalRequest> Requets { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

        public async Task OnGetAsync(int currentPage = 1)
        {
            IQueryable<WithdrawalRequest> requestQuery = _context.WithdrawalRequests.Include(p => p.Supporter).Include(p => p.Payment);

            if (!string.IsNullOrEmpty(Username))
            {
                requestQuery = requestQuery.Where(p => p.Supporter.Username.Contains(Username));
            }

            if (RequestId.HasValue)
            {
                requestQuery = requestQuery.Where(p => p.WithdrawalRequestId == RequestId.Value);
            }

            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                requestQuery = requestQuery.Where(p => p.Status == StatusFilter);
            }

            var totalPaymentsCount = await requestQuery.CountAsync();
            TotalPages = (int)System.Math.Ceiling(totalPaymentsCount / (double)PageSize);

            Requets = await requestQuery
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            CurrentPage = currentPage;
        }

		public async Task<IActionResult> OnPostUpdateStatusAsync(int WithdrawalRequestId, string newStatus, string username, int? requestIdFilter, string statusFilter)
		{
			var request = await _context.WithdrawalRequests.FindAsync(WithdrawalRequestId);
			if (request != null)
			{
				request.Status = newStatus;
				await _context.SaveChangesAsync();
			}

			return RedirectToPage(new { Username = username, PaymentId = requestIdFilter, StatusFilter = statusFilter });
		}
	}
}
