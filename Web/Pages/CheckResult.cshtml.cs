using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Controllers;
using Web.DbConnection;
using Web.Models;

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

        private MarkReportServices _markReportServices { get; set; }

        public List<MarkReport> MarkReports { get; set; } = new List<MarkReport>();

        public string Status { get; set; } = "";
        public MarkReportReponse markReportReponse = new MarkReportReponse();

        public CheckResultModel(WebContext context, MarkReportServices markReportServices)
        {
            _context = context;
            _markReportServices = markReportServices;

        }

        public async Task OnGetAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            UserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
            MarkReports = _context.MarkReports.Where(x => x.MarkReportId == Id && x.UserId == UserId).ToList();
            Status = _context.Payments.FirstOrDefault(x => x.RelatedId == Id).Status;

            var mark_report = await _context.MarkReports.FirstOrDefaultAsync(p => p.MarkReportId == Id);
            if (mark_report != null)
            {
                var file = await _context.Multimedia.FirstOrDefaultAsync(p => p.MarkReportId == mark_report.MarkReportId);
                if (file != null)
                {
                    MarkReportRequest reportRequest = new MarkReportRequest
                    {
                        markReportId = mark_report.MarkReportId,
                        url = file.MultimediaUrl
                    };
                    markReportReponse = await _markReportServices.CalculateMark(reportRequest);
                }

            }

        }


    }
}
