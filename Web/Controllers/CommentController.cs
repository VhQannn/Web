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
        private readonly IHubContext<PostHub> _postHub;
        public CommentController(WebContext context, IHubContext<PostHub> postHub)
        {
            _context = context;
            _postHub = postHub;
        }

        [HttpGet]
        public IActionResult GetCommentsByPost(int postId)
        {
            var parentComments = _context.ParentComments
                .Where(pc => pc.PostId == postId)
                .Include(pc => pc.Comments)
                .ThenInclude(c => c.User)
                .Include(pc => pc.User)
                .OrderBy(pc => pc.Price)  // Sắp xếp comment theo giá từ cao xuống thấp
                .ToList();

            var result = parentComments.Select(pc => new
            {
                pc.ParentCommentId,
                pc.Content,
                pc.Price,
                pc.CommentDate,
                ParentCommentUser = pc.User?.Username ?? "Unknown",
                ParentCommentUserId = pc.User?.UserId ?? 0,
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
            var userName = User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

            if (currentUser == null)
            {
				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
			}

            var post = await _context.Posts.Include(pc => pc.User).FirstOrDefaultAsync(m => m.PostId == postId);
            if (post == null)
            {
                return NotFound();
            }

            if (currentUser.UserType == "Customer" && post.User.UserId != currentUser.UserId)
            {
                return BadRequest("Khách không được comment hỗ trợ người khác.");
            }

            var existingComment = await _context.ParentComments.FirstOrDefaultAsync(c => c.PostId == postId && c.User.Username == userName);
            if (existingComment != null)
            {
                return BadRequest("Người dùng chỉ được phép comment 1 parent-comment.");
            }

            if (!decimal.TryParse(input.Price, out var price))
            {
                return BadRequest("Lỗi định dạng giá offer.");
            }

            var newComment = new ParentComment
            {
                PostId = postId,
                Content = input.Content,
                Price = price,
                UserId = currentUser.UserId,
                CommentDate = DateTime.Now
            };

            if (currentUser.UserId == post.UserId)
            {
                var notification = new Notification
                {
                    Message = "You got new comment in post: " + post.PostTitle,
                    CreatedAt = DateTime.Now,
                    RedirectUrl = "/Postdetail?Id=" + postId,
                    IsRead = false,
                    UserId = (int)post.UserId
                };
            }

            _context.ParentComments.Add(newComment);
            await _context.SaveChangesAsync();

            // Notify all clients about the new comment
            await _postHub.Clients.All.SendAsync("NewComment");

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
            var userName = User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
            if (currentUser == null)
            {
				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
			}

            var parentComment = await _context.ParentComments.Include(pc => pc.User).FirstOrDefaultAsync(m => m.ParentCommentId == parentCommentId);
            if (parentComment == null)
            {
                return NotFound("Comment gốc không tồn tại.");
            }

            if (parentComment.User.UserId != currentUser.UserId || currentUser.UserType != "Customer")
            {
                var post = await _context.Posts.Include(pc => pc.User).FirstOrDefaultAsync(m => m.PostId == parentComment.PostId);
                if (post == null)
                {
                    return NotFound("Bài viết không tồn tại.");
                }

                if (currentUser.UserId == parentComment.User.UserId)
                {
                    var childComment2 = new Comment
                    {
                        ParentCommentId = parentCommentId,
                        Content = content,
                        UserId = currentUser.UserId,
                        CommentDate = DateTime.Now
                    };

                    _context.Comments.Add(childComment2);
                    await _context.SaveChangesAsync();

                    return Ok();
                }

                if (post.User.UserId != currentUser.UserId)
                {
                    return BadRequest("Không được comment vào comment của người hỗ trợ khác.");
                }
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
