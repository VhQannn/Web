using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json.Linq;
using Web.DbConnection;
using Microsoft.AspNetCore.SignalR;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
	[Route("api/payment")]
	[ApiController]
	public class PaymentController : Controller
	{
		private readonly WebContext _context;
		private readonly IHubContext<NotificationHub> _notificationHub;
		public PaymentController(WebContext context, IHubContext<NotificationHub> notificationHub)
		{
			_context = context;
			_notificationHub = notificationHub;
		}


		[HttpPost]
		public async Task<IActionResult> HandlePaymentWebhook()
		{
			// 1. Kiểm tra header Secure-Token
			var receivedToken = Request.Headers["Secure-Token"];
			if (string.IsNullOrEmpty(receivedToken))
			{
				return BadRequest("Secure-Token header is missing.");
			}

			var expectedToken = "quangqui"; // Replace with your actual token
			if (!string.Equals(receivedToken, expectedToken, StringComparison.Ordinal))
			{
				return Unauthorized("Invalid Secure-Token header.");
			}

			using (var reader = new StreamReader(Request.Body))
			{
				var jsonString = await reader.ReadToEndAsync();

				// Phân tích và xử lý chuỗi JSON
				JObject jsonData = JObject.Parse(jsonString);
				var error = jsonData["error"].Value<int>();
				if (error == 0)
				{
					var transactionData = jsonData["data"].FirstOrDefault();
					if (transactionData != null)
					{
						var description = transactionData["description"].Value<string>();
						var amount = transactionData["amount"].Value<decimal>();

						// Tách 'Payment OrderID' để lấy paymentId
						var paymentOrderIdPrefix = "Payment OrderID";
						var start = description.IndexOf(paymentOrderIdPrefix);
						if (start != -1)
						{
							// Tách và lấy paymentId từ description
							var paymentIdStr = description.Substring(start + paymentOrderIdPrefix.Length).Trim();
							// Lấy các chữ số tiếp theo cho đến khi gặp ký tự không phải là số
							var paymentId = new string(paymentIdStr.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());

							if (int.TryParse(paymentId, out int paymentIdInt))
							{
								// Nếu chuyển đổi thành công, tiếp tục với xác thực và cập nhật
								var updateResult = await ValidateAndUpdatePayment(paymentIdInt, amount);
								if (!updateResult)
								{
									
								}
							}
							else
							{
								
							}
						}
					}
				}
			}

			return Ok(new { success = true });
		}



		public async Task<Payment> GetPaymentByIdAsync(int paymentId)
		{
			return await _context.Payments
								 .AsNoTracking()
								 .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
		}

		public async Task<bool> ValidateAndUpdatePayment(int paymentId, decimal receivedAmount)
		{
			var payment = await GetPaymentByIdAsync(paymentId);
			if (payment == null)
			{
				// Không tìm thấy payment
				return false;
			}

			if (payment.Amount != receivedAmount)
			{
				// Số tiền không khớp
				return false;
			}

			// Cập nhật trạng thái payment
			payment.Status = "COMPLETED";
			_context.Payments.Update(payment);
			await _context.SaveChangesAsync();
			var notifyHub = (IHubContext<NotificationHub>)HttpContext.RequestServices.GetService(typeof(IHubContext<NotificationHub>));
			await notifyHub.Clients.All.SendAsync("ProcessPayment");
			return true;
		}


		[HttpGet("check")]
		[Authorize]
		public async Task<IActionResult> CheckPaymentExists(int postId)
		{
			var paymentExists = await _context.Payments.FirstOrDefaultAsync(p => p.RelatedId == postId && p.ServiceType == "Post");
			return Ok(paymentExists);
		}
	}
}
