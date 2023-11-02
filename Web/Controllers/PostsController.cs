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
			var totalRecords = _context.Posts.Count();
			var skip = (pageNumber - 1) * pageSize;

			var posts = _context.Posts.Include(p => p.PostCategory).Include(p => p.User).Skip(skip).Take(pageSize).Select(p => new
			{
				postTitle = p.PostTitle,
				postContent = p.PostContent,
				postDate = p.PostDate,
				dateSlot = p.DateSlot,
				timeSlot = p.TimeSlot,
				status = p.Status,
				postCategoryName = p.PostCategory.PostCategoryName,
				username = p.User.Username,
				postId = p.PostId
			}).ToList();
			int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

			return Ok(new { data = posts, totalRecords, totalPages });
		}



	}
}
