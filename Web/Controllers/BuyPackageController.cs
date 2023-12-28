using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.DbConnection;
using Web.IRepository;
using Web.Models;

namespace Web.Controllers
{
	[Route("api/buy-package")]
	[ApiController]
	public class BuyPackageController : Controller
	{
		private readonly WebContext _context;
		public BuyPackageController(WebContext context)
		{
			_context = context;
		}
		[HttpPost]
		public IActionResult Post([FromForm] int packageId)
		{
			var currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return Unauthorized("Vui lòng đăng nhập để thực hiện thao tác!");
			}

			var package = _context.SupporterInsurancePackages.FirstOrDefault(u => u.PackageId == packageId);

			if (package == null)
			{
				return NotFound("Không tìm thấy gói dịch vụ!");
			}

			var payment = new Payment
			{
				UserId = currentUser.UserId,
				Amount = package.Price,
				PaymentDate = DateTime.UtcNow,
				ReceiverId = currentUser.UserId,
				RelatedId = package.PackageId,
				ServiceType = "InsurancePackage",
				Status = "PENDING"
			};

			_context.Payments.Add(payment);
			_context.SaveChanges();

			return Ok(new { success = true });
		}

	}
}
