using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.DbConnection;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WebContext _context;
        private readonly ILogger<IndexModel> _logger;
        public string? SelectedPostCategoryName { get; set; }
        public string hihi { get; set; }
        [BindProperty]
        public string Title { get; set; }
        public IndexModel(ILogger<IndexModel> logger, WebContext context)
        {
            _logger = logger;
            _context = context;
        }
        public List<Post> Post { get; set; } = default!;

        public void OnGetSearchByTitle(string title)
        {
            Post = _context.Posts.Include(p => p.PostCategory).Include(u => u.User)
                .Where(p => p.PostTitle.Contains(title)).ToList();
        }

        public async Task OnGetAsync(string? CategoryName = null)
        {
            ViewData["CategoryName"] = new SelectList(_context.PostCategories, "PostCategoryName", "PostCategoryName", SelectedPostCategoryName);
            Post = new List<Post>();
           
            if (!string.IsNullOrEmpty(CategoryName))
            {
                Console.Write(CategoryName);
            Post = _context.Posts.Include(p => p.PostCategory).Include(u => u.User).Where(p => p.PostCategory.PostCategoryName.Equals(CategoryName)).ToList();
            }
        }

    }
}