using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Shared.Components.Package
{
    public class DefaultModel : PageModel
    {
        public void OnGet()
        {
        }

		public IActionResult OnPostProcessPackage(int packageId)
		{
			// Lấy thông tin người dùng hiện tại
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Thực hiện logic xử lý tại đây

			// Trả về kết quả
			return new JsonResult(new { Success = true, PackageId = packageId });
		}
	}
}
