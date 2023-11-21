using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Services;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
	private readonly EmailService _emailService;

	public EmailController(EmailService emailService)
	{
		_emailService = emailService;
	}

	[HttpPost("send")]
	public async Task<IActionResult> SendEmail()
	{
		try
		{
			await _emailService.SendEmailAsync("qanvo313@gmail.com", "Greeting", "Hello");
			return Ok();

		}
		catch (Exception)
		{

			return NotFound();
		}
	}
}
