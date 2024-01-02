using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
	public class WithdrawalRequest : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
