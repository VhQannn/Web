using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.DbConnection;
using Web.Models; // Adjust this using directive based on your actual model namespace

namespace Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageTemplatesModel : PageModel
    {
        private readonly WebContext _context;

        public ManageTemplatesModel(WebContext context)
        {
            _context = context;
            Templates = new List<QuestionTemplate>();
        }

        public IList<QuestionTemplate> Templates { get; set; }

        public async Task OnGetAsync()
        {
            Templates = await _context.QuestionTemplates.ToListAsync() ?? new List<QuestionTemplate>();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int templateId)
        {
            var questionTemplate = await _context.QuestionTemplates
                              .FirstOrDefaultAsync(qt => qt.QuestionTemplateId == templateId);

            if (questionTemplate == null)
            {
                ViewData["ErrorMessage"] = "Template not found.";
                return Page();
            }

            // First, delete related QuestionTemplateDetailQaids
            var relatedQaids = await _context.QuestionTemplateDetailQaids
                                    .Where(q => q.QuestionTemplatesDetail.QuestionTemplateId == questionTemplate.QuestionTemplateId)
                                    .ToListAsync();
            _context.QuestionTemplateDetailQaids.RemoveRange(relatedQaids);

            // Then, delete related Multimedia
            var relatedMultimedia = await _context.Multimedia
                                        .Where(m => m.QuestionTemplatesDetailId != null && m.QuestionTemplatesDetail.QuestionTemplateId == questionTemplate.QuestionTemplateId)
                                        .ToListAsync();
            _context.Multimedia.RemoveRange(relatedMultimedia);

            // Next, delete related QuestionTemplatesDetails
            var relatedDetails = await _context.QuestionTemplatesDetails
                                    .Where(d => d.QuestionTemplateId == questionTemplate.QuestionTemplateId)
                                    .ToListAsync();
            _context.QuestionTemplatesDetails.RemoveRange(relatedDetails);

            // Finally, delete the QuestionTemplate
            _context.QuestionTemplates.Remove(questionTemplate);

            // Save all changes and reload Templates
            await _context.SaveChangesAsync();
            Templates = await _context.QuestionTemplates.ToListAsync();

            return RedirectToPage();
        }
    }
}
