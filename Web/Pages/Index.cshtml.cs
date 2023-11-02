using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Web.DbConnection.WebscamContext _context;
        private readonly ILogger<IndexModel> _logger;
        public string? SelectedPostCategoryName { get; set; }
        public IndexModel(ILogger<IndexModel> logger, Web.DbConnection.WebscamContext context)
        {
            _logger = logger;
            _context = context;
        }
        public List<Post> Post { get; set; } = default!;


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