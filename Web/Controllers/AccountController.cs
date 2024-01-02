using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.DTOs;
using Web.IRepository;
using Web.Models;

namespace Web.Controllers
{
    [Route("api/account")]
	[ApiController]
	public class AccountController : Controller
	{
		private readonly WebContext _context;
		private readonly IHubContext<NotificationHub> _notificationHub;
		private readonly MarkReportServices _markReportServices;
		private readonly IUserRepository _userRepositoy;
		public AccountController(WebContext context, IHubContext<NotificationHub> notificationHub, MarkReportServices markReportServices, IUserRepository userRepository)
		{
			_context = context;
			_notificationHub = notificationHub;
			_markReportServices = markReportServices;
			_userRepositoy = userRepository;
		}

		[HttpGet("current")]
		[Authorize]
		public IActionResult GetCurrentUserInfo()
		{
			var currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
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
				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
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
		public async Task<IActionResult> OnGetMyPayment(int pageNumber = 1, int pageSize = 5)
		{
			if (_context.Payments == null)
			{
				return NotFound("No payment data available.");
			}

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			IQueryable<Payment> query;

			if (user.UserType == "Supporter")
			{
				query = _context.Payments
						.Where(p => p.ReceiverId == user.UserId)
						.Include(p => p.User);
			}
			else
			{
				query = _context.Payments
						.Where(p => p.UserId == user.UserId)
						.Include(p => p.User);
			}

			var totalRecords = await query.CountAsync();
			var skip = (pageNumber - 1) * pageSize;
			var payments = await query.OrderByDescending(p => p.PaymentDate)
									 .Skip(skip)
									 .Take(pageSize)
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
										  },
										  Receiver = _context.Users.Where(u => u.UserId == p.ReceiverId).Select(r => new AccountDTO
										  {
											  Id = r.UserId,
											  Username = r.Username
										  }).FirstOrDefault()

									  })
									 .ToListAsync();

			int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

			return Ok(new { data = payments, totalRecords, totalPages });

		}


		[HttpGet("check-score-detail")]
		[Authorize]
		public async Task<IActionResult> OnGetCheckScoreDetail(int paymentId)
		{
			if (_context.Payments == null)
			{
				return NotFound("No payment data available.");
			}


			var payment = await _context.Payments
								 .AsNoTracking()
								 .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
			if (payment == null)
			{
				return NotFound("Payment not found.");
			}

			if (payment.ServiceType != "Check-Score")
			{
				return NotFound("Payment is not check-score service.");
			}

			var mark_report = await _context.MarkReports.FirstOrDefaultAsync(p => p.MarkReportId == payment.RelatedId);

			if (mark_report == null)
			{
				return NotFound("Mark report not found.");
			}

			string response;
			var timeString = mark_report.CreatedDate.ToString("HH:mm:ss");
			var file = await _context.Multimedia.FirstOrDefaultAsync(p => p.MarkReportId == mark_report.MarkReportId);

			if (file != null)
			{
				var subjectCode = await _markReportServices.GetTemplateCodeFromLink(file.MultimediaUrl);

				response = $"Thời gian: {timeString}\nMã môn: {subjectCode}";
			}
			else
			{
				response = $"Không tìm thấy file";
			}

			return Ok(new { Message = response });

		}

		[AllowAnonymous]
		[HttpGet("verify")]
		public async Task<ActionResult> VerifyEmail(string email, string username, string otp)
		{
			try
			{
				//check otp nếu đúng thì đổi
				var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(email) && x.Username == username);
				if (DateTime.Now - user.OtpcreateTime > TimeSpan.FromMinutes(5))
				{
					return NotFound("Mã OTP đã hết hạn.");
				}
				if (user.Otpcode.Equals(otp))
				{
					user.IsVerify = true;
					var check = await _context.SaveChangesAsync() > 0;
					if (check)
					{
                        return Redirect("/verifyemail");
                    }
					return NotFound("Không thể xác thực");

				}
				return NotFound("Mã OTP không chính xác");
			}
			catch (Exception)
			{

				return NotFound("Không thể xác thực");
			}
		}

		[Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ForgetPassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
				var check = await _userRepositoy.ChangePassword(changePasswordDTO.Username, changePasswordDTO.Password);
				if(check != null)
				{
					return Ok(check);
				}
				return BadRequest();
            }
            catch (Exception)
            {

                return NotFound("Không thể xác thực");
            }
        }

		[Authorize]
		[HttpPost("coin-payment")]
		public async Task<ActionResult> PaymentByVirtualCurrency([FromBody] PaymentRequestDTO paymentRequest)
		{
			var currentUser = await _context.Users
		.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);

			if (currentUser == null)
			{
				return Unauthorized("Vui lòng đăng nhập.");
			}

			var payment = await _context.Payments
				.FirstOrDefaultAsync(p => p.PaymentId == paymentRequest.PaymentId && p.UserId == currentUser.UserId);

			if (payment == null)
			{
				return NotFound("Không tìm thấy thông tin thanh toán.");
			}

			if (currentUser.VirtualCurrencyBalance < payment.Amount)
			{
				return BadRequest("Số dư không đủ để thực hiện giao dịch.");
			}

			// Xử lý giảm số dư tiền ảo
			currentUser.VirtualCurrencyBalance -= payment.Amount;

			// Thêm lịch sử giao dịch tiền ảo
			var transaction = new VirtualCurrency
			{
				UserId = currentUser.UserId,
				Amount = -payment.Amount,
				TransactionType = "Payment",
				TransactionDate = DateTime.UtcNow
			};
			_context.VirtualCurrencies.Add(transaction);

			// Cập nhật trạng thái thanh toán
			payment.Status = "COMPLETED";

			await _context.SaveChangesAsync();

			return Ok(new { message = "Thanh toán thành công." });
		}


		[HttpGet("balance")]
		[Authorize]
		public async Task<IActionResult> GetBalance()
		{
			var currentUserName = User.Identity.Name;
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == currentUserName);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			return Ok(new { Balance = user.VirtualCurrencyBalance });
		}
	}
}
