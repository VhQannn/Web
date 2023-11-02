using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly WebscamContext _context;

        public CommentController(WebscamContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCommentsByPost(int postId)
        {
            var parentComments = _context.ParentComments
                .Where(pc => pc.PostId == postId)
                .Include(pc => pc.Comments)
                .ThenInclude(c => c.User)
                .Include(pc => pc.User)
                .ToList();

			var result = parentComments.Select(pc => new
			{
				pc.ParentCommentId,
				pc.Content,
				pc.CommentDate,
				ParentCommentUser = pc.User?.Username ?? "Unknown",
				Comments = pc.Comments.Select(c => new
				{
					c.Content,
					c.CommentDate,
					User = c.User?.Username ?? "Unknown"
				})
			});


			return Ok(result);
        }


		[HttpPost]
		public async Task<IActionResult> AddComment(int postId, [FromBody] string content)
		{
			// Tạo một instance mới cho ParentComment
			var newComment = new ParentComment
			{
				PostId = postId,
				Content = content,
                UserId = 1,
				CommentDate = DateTime.Now
			};

			_context.ParentComments.Add(newComment);
			await _context.SaveChangesAsync();

			return Ok(newComment);
		}

		[HttpPost("reply")]
		public async Task<IActionResult> AddChildComment(int parentCommentId, [FromBody] string content)
		{
			var childComment = new Comment
			{
				ParentCommentId = parentCommentId,
				Content = content,
				UserId = 1,
				CommentDate = DateTime.Now
			};

			_context.Comments.Add(childComment);
			await _context.SaveChangesAsync();

			return Ok(childComment);
		}



	}

}
