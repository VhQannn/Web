using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Web.DbConnection;

namespace Web.Pages
{
    [Authorize]
    public class CheckResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Uri { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public int UserId { get; set; }

        private WebContext _context { get; set; }

        public List<MarkReport> MarkReports { get; set; } = new List<MarkReport>();

        public string Status { get; set; } = ""; 

        public CheckResultModel(WebContext context)
        {
            _context = context;

        }

        public void OnGet()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            UserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
            MarkReports = _context.MarkReports.Where(x => x.MarkReportId == Id && x.UserId == UserId).ToList();
            Status = _context.Payments.FirstOrDefault(x => x.RelatedId == Id).Status;
        }
    }
}
