using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.DbConnection;
using Web.Models;

namespace Web.Controllers
{
	[Route("api/account")]
	[ApiController]
	public class AccountController : Controller
	{
		private readonly WebContext _context;

		public AccountController(WebContext context)
		{
			_context = context;
		}

		[HttpGet("current")]
		[Authorize]
		public IActionResult GetCurrentUserInfo()
		{
			var currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound("Người dùng hiện tại không được tìm thấy trong cơ sở dữ liệu.");
			}

			var userDTO = new AccountDTO
			{
				Id = currentUser.UserId,
				Username = currentUser.Username,
				Role = currentUser.UserType != null ? currentUser.UserType : "Unknown"
			};

			return Ok(userDTO);
		}


	}
}
