using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetAllPosts()
        {
            var posts = _context.Posts.Select(p => new
            {
                postTitle = p.PostTitle,
                postContent = p.PostContent,
                postDate = p.PostDate,
                timeSlot = p.TimeSlot,
                status = p.Status,
                postCategoryName = p.PostCategory.PostCategoryName,
                username = p.User.Username,
                postId = p.PostId
            }).ToList();

            return Ok(posts);
        }

    }
}
