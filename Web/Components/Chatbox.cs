using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
	public class Chatbox : ViewComponent
	{
		public IViewComponentResult Invoke() {
			return View(); 
		}
	}
}
