//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using Web.DbConnection;
//using Web.Models;

//namespace Web.Controllers
//{
//    [Route("api/withdrawal-requests")]
//	[ApiController]
//	public class WithdrawalRequestsController : ControllerBase
//	{
//		private readonly WebContext _context;
//		private readonly IHubContext<NotificationHub> _notificationHub;
//		public WithdrawalRequestsController(WebContext context, IHubContext<NotificationHub> notificationHub)
//		{
//			_context = context;
//            _notificationHub = notificationHub;
//		}

//		// POST: api/WithdrawalRequests/Create
//        [HttpPost("create")]
//        public async Task<IActionResult> CreateWithdrawalRequest([FromBody] WithdrawalRequestDTO requestDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var requestExits = _context.WithdrawalRequests.FirstOrDefault(u => u.PaymentId == requestDto.PaymentId);
//            if(requestExits != null)
//            {
//				return BadRequest("This payment request is already exits");
//			}

//			var currentUserName = User.Identity.Name;
//			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

//			if (currentUser == null)
//			{
//				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
//			}

//            var paymentRequest = _context.Payments.FirstOrDefault(u => u.PaymentId == requestDto.PaymentId);

//            if (paymentRequest == null)
//            {
//                return BadRequest("Payment not found");
//            }

//			if (currentUser.UserType != "Supporter")
//            {
//				return BadRequest("Just Supporter can withdrawal request.");
//			}

//            if(currentUser.UserId != paymentRequest.ReceiverId)
//            {
//				return BadRequest("Don't permission to request this payment.");
//			}

//			var withdrawalRequest = new WithdrawalRequest
//            {
//                // Map from DTO to WithdrawalRequest entity
//                PaymentId = requestDto.PaymentId,
//                SupporterId = currentUser.UserId,
//                Status = "pending",
//                Comments = requestDto.Comments
//            };

//            _context.WithdrawalRequests.Add(withdrawalRequest);
//            await _context.SaveChangesAsync();
//            await _notificationHub.Clients.Group(currentUser.Username).SendAsync("NewWithdrawalRequest");
//            return Ok(new { Message = "Withdrawal request created successfully.", RequestId = withdrawalRequest.WithdrawalRequestId });
//        }

//	}
//}
