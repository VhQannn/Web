using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.Models;

namespace Web.Controllers
{
	[Route("api/posts")]
	[ApiController]
	public class PostsController : Controller
	{
		private readonly WebContext _context;
		private readonly IHubContext<PostHub> _postHub;
		private readonly ILogger<PaymentController> _logger;

		public PostsController(WebContext context, IHubContext<PostHub> postHub, ILogger<PaymentController> logger)
		{
			_context = context;
			_postHub = postHub;
			_logger = logger;
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


		[HttpGet("get-by-title")]
		public IActionResult GetPostByTitle(string title)
		{
			var totalRecords = _context.Posts.Count();

			var posts = _context.Posts.Include(p => p.PostCategory).Include(p => p.User).Select(p => new
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
			}).Where(x => x.postTitle.Contains(title) || x.postCategoryName.Contains(title)).ToList();

			return Ok(new { data = posts });
		}

		[HttpPost("update-status-for-supporter")]
		[Authorize]
		public async Task<IActionResult> UpdatePostStatus(UpdatePostStatusForSupporter postRequest)
		{
			var currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
			}

			if (postRequest.ServiceType == "Post")
			{
				var post = _context.Posts.FirstOrDefault(p => p.PostId == postRequest.RelatedId);
				if (post == null)
				{
					return BadRequest("Không tìm thấy dịch vụ cần cập nhật");
				}

				if (post.ReceiverId != currentUser.UserId)
				{
					return BadRequest("Chỉ có người nhận bài post mới được cập nhật trạng thái đã xong");
				}



				post.Status = "COMPLETED";
				_context.Posts.Update(post);
				await _context.SaveChangesAsync();
				await _postHub.Clients.All.SendAsync("UpdatePosts");
				return Ok("Bài đăng đã được cập nhật thành trạng thái");
			}

			// Xử lý cho các loại ServiceType khác nếu cần
			return BadRequest("Loại dịch vụ không hợp lệ hoặc chưa được xử lý");
		}

	}
}