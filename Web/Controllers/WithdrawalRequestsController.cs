using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.Models;

namespace Web.Controllers
{
    [Route("api/withdrawal-requests")]
	[ApiController]
	public class WithdrawalRequestsController : ControllerBase
	{
		private readonly WebContext _context;
		private readonly IHubContext<NotificationHub> _notificationHub;
		public WithdrawalRequestsController(WebContext context, IHubContext<NotificationHub> notificationHub)
		{
			_context = context;
            _notificationHub = notificationHub;
		}

		// POST: api/WithdrawalRequests/Create
		[HttpPost("create")]
		public async Task<IActionResult> CreateWithdrawalRequest([FromBody] WithdrawalRequestDTO requestDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var currentUserName = User.Identity.Name;

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == currentUserName);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			// Kiểm tra số dư hiện tại của người dùng
			if (user.VirtualCurrencyBalance < requestDto.Amount)
			{
				return BadRequest("Insufficient balance.");
			}

			var withdrawalRequest = new WithdrawalRequest
			{
				UserId = user.UserId,
				Amount = requestDto.Amount,
				RequestDate = DateTime.UtcNow,
				Status = "Pending",
				Comments = requestDto.Comments
			};

			_context.WithdrawalRequests.Add(withdrawalRequest);
			// Trừ số dư hiện tại của người dùng
			user.VirtualCurrencyBalance -= requestDto.Amount;

			await _context.SaveChangesAsync();
			await _notificationHub.Clients.Group(user.Username).SendAsync("NewWithdrawalRequest");

			return Ok(new { Message = "Withdrawal request created successfully.", RequestId = withdrawalRequest.WithdrawalRequestId });
		}


	}
}
