using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Web.DbConnection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Web.Models;

namespace Web.Controllers
{
	[Route("api/payment")]
	[ApiController]
	public class PaymentController : Controller
	{
		private readonly WebContext _context;
		private readonly IHubContext<NotificationHub> _notificationHub;
		private readonly ILogger<PaymentController> _logger;
        private readonly MarkReportServices _markReportServices;

        public PaymentController(WebContext context, IHubContext<NotificationHub> notificationHub, ILogger<PaymentController> logger, MarkReportServices markReportServices)
        {
            _context = context;
            _notificationHub = notificationHub;
            _logger = logger;
            _markReportServices = markReportServices;
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

			// Nên lấy expectedToken từ biến môi trường hoặc cấu hình an toàn
			var expectedToken = "quangqui";
			if (!string.Equals(receivedToken, expectedToken, StringComparison.Ordinal))
			{
				return Unauthorized("Invalid Secure-Token header.");
			}

			JObject jsonData;
			using (var reader = new StreamReader(Request.Body))
			{
				var jsonString = await reader.ReadToEndAsync();
				try
				{
					jsonData = JObject.Parse(jsonString);
				}
				catch (JsonReaderException ex)
				{
					_logger.LogError(ex, "Invalid JSON format");
					return BadRequest("Invalid JSON format.");
				}
			}

			// Phân tích và xử lý chuỗi JSON sau khi xác minh định dạng
			try
			{
				_logger.LogInformation("Payment webhook received: {jsonData}", jsonData);
				var error = jsonData["error"].Value<int>();
				if (error == 0)
				{
					var transactionData = jsonData["data"].FirstOrDefault();
					if (transactionData != null)
					{
						var description = transactionData["description"].Value<string>();
						var amount = transactionData["amount"].Value<decimal>();

						var paymentOrderIdPrefix = "Payment OrderID";
						var start = description.IndexOf(paymentOrderIdPrefix);
						if (start != -1)
						{
							var paymentIdStr = description.Substring(start + paymentOrderIdPrefix.Length).Trim();
							var paymentId = new string(paymentIdStr.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());

							if (int.TryParse(paymentId, out int paymentIdInt))
							{
								var updateResult = await ValidateAndUpdatePayment(paymentIdInt, amount);
								if (!updateResult)
								{
									_logger.LogWarning("Failed to update payment status for payment ID: {paymentIdInt}", paymentIdInt);
								}
							}
							else
							{
								_logger.LogWarning("Failed to parse payment ID from description.");
							}
						}
						else
						{
							_logger.LogWarning("Payment not related to this system.");
						}
					}
					else
					{
						_logger.LogWarning("No transaction data found in the webhook JSON.");
						return BadRequest("No transaction data found.");
					}
				}
				else
				{
					_logger.LogWarning("Webhook JSON contained an error code: {error}", error);
					return BadRequest($"Webhook JSON contained an error code: {error}");
				}
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing payment webhook");
				return StatusCode(500, "Internal server error occurred while processing payment.");
			}
		}


		public async Task<Payment> GetPaymentByIdAsync(int paymentId)
		{
			return await _context.Payments
								 .AsNoTracking()
								 .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
		}

		public async Task<bool> ValidateAndUpdatePayment(int paymentId, decimal receivedAmount)
		{
			_logger.LogInformation($"Starting validation for payment ID: {paymentId} with received amount: {receivedAmount}.");
			var payment = await GetPaymentByIdAsync(paymentId);
			if (payment == null)
			{
				_logger.LogWarning($"Payment ID: {paymentId} not found.");
				return false;
			}

			if (payment.Amount != receivedAmount)
			{
				_logger.LogWarning($"Received amount {receivedAmount} does not match expected amount {payment.Amount} for payment ID: {paymentId}.");
				return false;
			}

			_logger.LogInformation($"Updating payment status to 'COMPLETED' for payment ID: {paymentId}.");
			payment.Status = "COMPLETED";
			_context.Payments.Update(payment);

			if (payment.ServiceType == "Post")
			{
				var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == payment.RelatedId);
				if (post != null)
				{
					_logger.LogInformation($"Updating post with ID: {post.PostId} to have receiver ID: {payment.ReceiverId}.");
					post.ReceiverId = payment.ReceiverId;
					_context.Posts.Update(post); // Cập nhật post trong context
				}
				else
				{
					_logger.LogWarning($"Post related to payment ID: {paymentId} not found.");
				}
			}
            else if(payment.ServiceType == "Check-Score")

            {
				var mark_report = await _context.MarkReports.FirstOrDefaultAsync(p => p.MarkReportId == payment.RelatedId);
				if(mark_report != null)
				{
                    _logger.LogInformation($"Processing calcualate score with ID: {mark_report.MarkReportId} to have receiver ID: {payment.ReceiverId}.");
                    var file = await _context.Multimedia.FirstOrDefaultAsync(p => p.MarkReportId == mark_report.MarkReportId);
					if(file != null)
					{
						MarkReportRequest reportRequest = new MarkReportRequest
						{
							markReportId = mark_report.MarkReportId,
							url = file.MultimediaUrl
                        };
                       await _markReportServices.CalculateMark(reportRequest);
                    }

                }

            }


			try
			{
				await _context.SaveChangesAsync();
				_logger.LogInformation($"Payment ID: {paymentId} status updated successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error updating payment ID: {paymentId} status.");
				return false;
			}

			try
			{
				_logger.LogInformation($"Notifying clients about the payment status update for payment ID: {paymentId}.");
				await _notificationHub.Clients.All.SendAsync("ProcessPayment");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error sending notification for payment ID: {paymentId}.");
			}

			return true;
		}


		[HttpGet("check")]
		[Authorize]
		public async Task<IActionResult> CheckPaymentExists(int postId)
		{
			var paymentExists = await _context.Payments.FirstOrDefaultAsync(p => p.RelatedId == postId && p.ServiceType == "Post");
			return Ok(paymentExists);
		}
		
		[HttpGet("check-score")]
		[Authorize]
		public async Task<IActionResult> CheckPaymentScore(int markReportId)
		{
			var paymentExists = await _context.Payments.FirstOrDefaultAsync(p => p.RelatedId == markReportId && p.ServiceType == "Check-Score");
			return Ok(paymentExists);
		}
	}
}
