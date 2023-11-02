using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Web.DbConnection;

namespace Web.Pages
{
    [Authorize]
    public class CreatePostModel : PageModel
    {
        private readonly WebscamContext _context;

        public CreatePostModel(WebscamContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "PostCategoryId", "PostCategoryName");
            return Page();
        }

        [BindProperty]
        public Post Post { get; set; } = default!;

        [BindProperty]
        public string StartTime { get; set; } = default!;

        [BindProperty]
        public string EndTime { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            Post.PostDate = DateTime.Now;
            Post.Status = "pending";
            Post.TimeSlot = $"{StartTime} - {EndTime}";

            if (!ModelState.IsValid || _context.Posts == null || Post == null)
            {
                return Page();
            }
            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
