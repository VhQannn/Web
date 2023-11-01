using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly WebContext _context;

        public CommentController(WebContext context)
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
				.OrderByDescending(pc => pc.Price)  // Sắp xếp comment theo giá từ cao xuống thấp
				.ToList();

			var result = parentComments.Select(pc => new
			{
				pc.ParentCommentId,
				pc.Content,
				pc.Price,
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
		public async Task<IActionResult> AddComment(int postId, [FromBody] CommentInputModel input)
		{
			// 1. Lấy tên đăng nhập của người dùng hiện tại
			var userName = User.Identity.Name;

			// 2. Truy vấn cơ sở dữ liệu để lấy đối tượng người dùng
			var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

			if (currentUser == null)
			{
				return BadRequest("Người dùng hiện tại không tồn tại trong cơ sở dữ liệu.");
			}

			decimal price;
			if (!decimal.TryParse(input.Price, out price))
			{
				return BadRequest("Invalid price format.");
			}

			// 3. Sử dụng ID của người dùng khi tạo bình luận
			var newComment = new ParentComment
			{
				PostId = postId,
				Content = input.Content,
				Price = price,
				UserId = currentUser.UserId,
				CommentDate = DateTime.Now
			};

			_context.ParentComments.Add(newComment);
			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("checkParentComment")]
		public async Task<IActionResult> CheckParentComment(int postId)
		{
			var userName = User.Identity.Name;
			var existingComment = await _context.ParentComments.FirstOrDefaultAsync(c => c.PostId == postId && c.User.Username == userName);
			return Ok(existingComment != null);
		}



		[HttpPost("reply")]
		public async Task<IActionResult> AddChildComment(int parentCommentId, [FromBody] string content)
		{
			// Lấy người dùng hiện tại và ID của họ tương tự như trên
			var userName = User.Identity.Name;
			var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
			if (currentUser == null)
			{
				return BadRequest("Người dùng hiện tại không tồn tại trong cơ sở dữ liệu.");
			}

			var childComment = new Comment
			{
				ParentCommentId = parentCommentId,
				Content = content,
				UserId = currentUser.UserId,
				CommentDate = DateTime.Now
			};

			_context.Comments.Add(childComment);
			await _context.SaveChangesAsync();

			return Ok();
		}


		[HttpPost("updatePrice")]
		public async Task<IActionResult> UpdatePrice(int commentId, [FromBody] UpdatePriceInputModel input)
		{
			// Lấy bình luận dựa trên commentId
			var commentToUpdate = await _context.ParentComments.FirstOrDefaultAsync(c => c.ParentCommentId == commentId);

			// Kiểm tra xem bình luận có tồn tại không
			if (commentToUpdate == null)
			{
				return NotFound("Bình luận không tồn tại.");
			}

			// Cập nhật giá
			commentToUpdate.Price = input.Price;

			// Lưu thay đổi
			await _context.SaveChangesAsync();

			return Ok(new { message = "Price updated successfully!" });
		}




	}

}
