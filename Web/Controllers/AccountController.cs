using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.Models;

namespace Web.Controllers
{
	[Route("api/account")]
	[ApiController]
	public class AccountController : Controller
	{
		private readonly WebContext _context;
		private readonly IHubContext<NotificationHub> _notificationHub;

		public AccountController(WebContext context, IHubContext<NotificationHub> notificationHub)
		{
			_context = context;
			_notificationHub = notificationHub;
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


		[HttpPost("create-payment")]
		[Authorize]
		public IActionResult CreatePayment([FromBody] PaymentDTO paymentDTO)
		{
			if (paymentDTO == null)
			{
				return BadRequest("Thông tin thanh toán không hợp lệ.");
			}

			var currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound("Người dùng hiện tại không được tìm thấy trong cơ sở dữ liệu.");
			}

			var payment = new Payment
			{
				UserId = currentUser.UserId,
				Amount = paymentDTO.Amount,
				PaymentDate = DateTime.UtcNow,
				ReceiverId = paymentDTO.ReceiverId,
				RelatedId = paymentDTO.RelatedId,
				ServiceType = paymentDTO.ServiceType,
				Status = paymentDTO.Status
			};

			_context.Payments.Add(payment);
			_context.SaveChanges();

			return Ok(new { PaymentId = payment.PaymentId });
		}

		[HttpGet("my-payment")]
		[Authorize]
		public async Task<IActionResult> OnGetMyPayment()
		{
			if (_context.Payments == null)
			{
				// If there are no payments, return a Not Found response.
				return NotFound("No payment data available.");
			}

			// Get the current user's ID
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
			if (user == null)
			{
				// If the user is not found, return a Not Found response.
				return NotFound("User not found.");
			}

			if (user.UserType == "Supporter")
			{
				// Filter the payments to only include those for the current user and order them by the most recent
				var payments = await _context.Payments
					.Where(p => p.ReceiverId == user.UserId)
					.OrderByDescending(p => p.PaymentDate) // This will sort the payments by date, most recent first
					.Include(p => p.User) // Including User entity if necessary.
					 .Select(p => new MyPaymentSupporterDTO // Project onto the DTO
					 {
						 PaymentId = p.PaymentId,
						 Amount = p.Amount,
						 PaymentDate = p.PaymentDate,
						 Status = p.Status,
						 RelatedId = p.RelatedId,
						 ServiceType = p.ServiceType,
						 User = new AccountDTO
						 {
							 Id = user.UserId,
							 Username = user.Username,
							 Role = user.UserType
						 }
						 ,
						 Receiver = new AccountDTO
						 {
							 Id = p.User.UserId,
							 Username = p.User.Username
						 },

						 WithdrawalRequest = _context.WithdrawalRequests.Where(a => a.PaymentId == p.PaymentId)
							.Select(r => new WithdrawalDTO
							{
								WithdrawalRequestId = r.WithdrawalRequestId,
								PaymentId = r.PaymentId,
								Comments = r.Comments,
								RequestDate = r.RequestDate,
								Status = r.Status
								
							}).FirstOrDefault()

					 })
					.ToListAsync();

				return Ok(payments);
			}
			else
			{
				// Filter the payments to only include those for the current user and order them by the most recent
				var payments = await _context.Payments
					.Where(p => p.UserId == user.UserId)
					.OrderByDescending(p => p.PaymentDate) // This will sort the payments by date, most recent first
					.Include(p => p.User) // Including User entity if necessary.
					 .Select(p => new MyPaymentDTO // Project onto the DTO
					 {
						 PaymentId = p.PaymentId,
						 Amount = p.Amount,
						 PaymentDate = p.PaymentDate,
						 Status = p.Status,
						 RelatedId = p.RelatedId,
						 ServiceType = p.ServiceType,
						 User = new AccountDTO
						 {
							 Id = p.User.UserId,
							 Username = p.User.Username
						 }
						 ,
						 Receiver = _context.Users.Where(a => a.UserId == p.ReceiverId)
							.Select(r => new AccountDTO
							{
								Id = r.UserId,
								Username = r.Username
							}).FirstOrDefault()
					 })
					.ToListAsync();

				return Ok(payments);
			}


		}


	}
}
