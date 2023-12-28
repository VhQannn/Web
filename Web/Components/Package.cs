using Microsoft.AspNetCore.Mvc;
using Web.DbConnection;
using Web.IRepository;

namespace Web.Components
{
    public class Package : ViewComponent
    {
		private readonly WebContext _context;
		private readonly IUserRepository _userRepository;
		public Package(IUserRepository userRepository, WebContext context)
		{
			_context = context;
			_userRepository = userRepository;
		}

		public IViewComponentResult Invoke()
		{
			var currentUserName = User.Identity.Name;


			var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return Content(string.Empty);
			}

			if(currentUser.UserType == "Supporter" || currentUser.UserType == "Customer")
			{

				var currentDate = DateTime.UtcNow;

				var hasActiveInsurance = _context.UserSupporterInsurances
					.Any(usi => usi.User.Username == currentUserName
								&& usi.StartDate <= currentDate
								&& usi.EndDate >= currentDate);

				if (!hasActiveInsurance)
				{
					// Cập nhật loại tài khoản người dùng
					_userRepository.UpdateRole(currentUser.UserId, "Customer");

					var packages = _context.SupporterInsurancePackages
										   .Select(p => new SupporterInsurancePackage
										   {
											   PackageId = p.PackageId,
											   PackageName = p.PackageName,
											   Duration = p.Duration,
											   Price = p.Price
										   }).ToList();

					return View(packages); // Trả về view với danh sách gói
				}
			}

			// Nếu người dùng đã mua gói bảo hiểm, không hiển thị gì
			return Content(string.Empty);
		}
	}
}
