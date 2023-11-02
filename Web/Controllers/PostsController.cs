using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : Controller
    {
        private readonly WebContext _context;

        public PostsController(WebContext context)
        {
            _context = context;
        }

		[HttpGet]
		public IActionResult GetAllPosts(int pageNumber = 1, int pageSize = 5)
		{
			var roles = HttpContext.Session.GetString("Role");

			var totalRecords = _context.Posts.Count();
			var skip = (pageNumber - 1) * pageSize;

            IQueryable<Post> query = _context.Posts.Include(p => p.PostCategory).Include(p => p.User);

            // Filter the posts based on user role
            if (roles == null || roles.Contains("Customer") || roles.Contains("Supporter") || roles.Contains("Seller"))
            {
                query = query.Where(p => p.Status != "pending");
            }

            var posts = query.Skip(skip).Take(pageSize).Select(p => new
            {
                postTitle = p.PostTitle,
                postContent = p.PostContent,
                postDate = p.PostDate,
                dateSlot = p.DateSlot,
                timeSlot = p.TimeSlot,
                status = p.Status,
                postCategoryName = p.PostCategory.PostCategoryName,
                username = p.Poster.Username,
                postId = p.PostId
            }).ToList();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return Ok(new { data = posts, totalRecords, totalPages });
        }
	}
}
