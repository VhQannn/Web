using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Web.DbConnection.WebscamContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, Web.DbConnection.WebscamContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IList<Post> Post { get; set; } = default!;


        public async Task OnGetAsync()
        {
            if (_context.Posts != null)
            {
                Post = await _context.Posts
                .Include(p => p.PostCategory)
                .Include(p => p.User).ToListAsync();
            }
        }
    }
}