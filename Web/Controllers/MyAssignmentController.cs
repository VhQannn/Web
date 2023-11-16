using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Web.DbConnection;
using Web.Models;

namespace Web.Controllers
{
	[Route("api/my-assignment")]
	[ApiController]
	public class MyAssignmentController : Controller
	{

		private readonly WebContext _context;
		private readonly IHubContext<PostHub> _postHub;
		private readonly ILogger<PaymentController> _logger;

		public MyAssignmentController(WebContext context, IHubContext<PostHub> postHub, ILogger<PaymentController> logger)
		{
			_context = context;
			_postHub = postHub;
			_logger = logger;
		}


		[HttpGet]
		public IActionResult GetAllPosts(int pageNumber = 1, int pageSize = 5)
		{
			var currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
			}

			IQueryable<Post> query;

			if (currentUser.UserType == "Supporter")
			{
				query = _context.Posts.Include(p => p.PostCategory).Include(p => p.User).Where(p => p.ReceiverId == currentUser.UserId);
			}
			else
			{
				query = _context.Posts.Include(p => p.PostCategory).Include(p => p.User).Where(p => p.ReceiverId != null && p.UserId == currentUser.UserId);
			}

			var totalRecords = query.Count();
			var skip = (pageNumber - 1) * pageSize;

			var posts = new List<MyAssignment>();


			if (currentUser.UserType == "Supporter")
			{
				posts = query.Skip(skip).Take(pageSize).Select(p => new MyAssignment
				{
					Poster = _context.Users.Where(a => a.UserId == p.UserId)
							.Select(r => new AccountDTO
							{
								Id = r.UserId,
								Role = r.UserType,
								Username = r.Username
							}).FirstOrDefault(),
					PostId = p.PostId,
					DateSlot = p.DateSlot,
					TimeSlot = p.TimeSlot,
					Status = p.Status,
					Rating = _context.Ratings.Where(a => a.RelatedId == p.PostId)
							.Select(r => new RatingDTO
							{
								RelatedId = r.RelatedId,
								ServiceType = r.ServiceType,
								Comments = r.Comments,
								RatingValue = r.RatingValue,
								RatingDate = r.RatingDate,
							}).FirstOrDefault()
				})
				.OrderByDescending(p => p.DateSlot)
				.ToList();
			}
			else
			{
				posts = query.Skip(skip).Take(pageSize).Select(p => new MyAssignment
				{
					Poster = _context.Users.Where(a => a.UserId == p.ReceiverId)
							.Select(r => new AccountDTO
							{
								Id = r.UserId,
								Role = r.UserType,
								Username = r.Username
							}).FirstOrDefault(),
					PostId = p.PostId,
					DateSlot = p.DateSlot,
					TimeSlot = p.TimeSlot,
					Status = p.Status,
					Rating = _context.Ratings.Where(a => a.RelatedId == p.PostId)
							.Select(r => new RatingDTO
							{
								RelatedId = r.RelatedId,
								ServiceType = r.ServiceType,
								Comments = r.Comments,
								RatingValue = r.RatingValue,
								RatingDate = r.RatingDate,
							}).FirstOrDefault()
				})
				.OrderByDescending(p => p.DateSlot)
				.ToList();
			}

			int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

			return Ok(new { data = posts, totalRecords, totalPages });
		}


		[HttpPost]
		public IActionResult CreateRating([FromBody] RatingDTO ratingDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound(new { message = "Người dùng hiện tại không được tìm thấy trong cơ sở dữ liệu." });
			}


			var post = _context.Posts.FirstOrDefault(p => p.PostId == ratingDTO.RelatedId);

			if (post == null)
			{
				return BadRequest(new { message = "Không tìm thấy dịch vụ cần cập nhật" });
			}

			if (post.UserId != currentUser.UserId)
			{
				return BadRequest(new { message = "Chỉ có người đăng bài post mới được đánh giá" });
			}

			var ratingExits = _context.Ratings.FirstOrDefault(r => r.RelatedId == post.PostId);
			if (ratingExits != null)
			{
				return BadRequest(new { message = "Đã có rating cho bài viết này" });
			}


			Rating rating = new Rating
			{
				RaterId = currentUser.UserId,
				SupporterId = ratingDTO.SupporterId,
				RelatedId= ratingDTO.RelatedId,
				ServiceType= ratingDTO.ServiceType,
				Comments = ratingDTO.Comments,
				RatingValue = ratingDTO.RatingValue,
				RatingDate = DateTime.UtcNow,
			};

			_context.Ratings.Add(rating);
			_context.SaveChanges();
			_postHub.Clients.All.SendAsync("UpdatePosts");

			return Ok(new { message = "Rating created successfully." });
		}
	}
}
