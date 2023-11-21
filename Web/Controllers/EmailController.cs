using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web.DbConnection;
using Web.DTOs;
using Web.Services;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
	private readonly EmailService _emailService;
	private readonly WebContext _context;
	public EmailController(EmailService emailService, WebContext context)
	{
		_emailService = emailService;
		_context = context;
	}

	public string GetOTP()
	{
		Random random = new Random();
		string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		char[] result = new char[6];
		for (int i = 0; i < 6; i++)
		{
			result[i] = characters[random.Next(characters.Length)];
		}
		string randomString = new string(result);
		return randomString;
	}

	[HttpPost("send-verify-email")]
	public async Task<IActionResult> VerifyEmail(VerifyEmailDTO verifyEmailDTO)
	{
		try
		{
			var otpCode = GetOTP();
			await _emailService.SendEmailAsync(verifyEmailDTO.Email, verifyEmailDTO.Username, otpCode, "Verify Email");
			var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(verifyEmailDTO.Email) && x.Username.Equals(verifyEmailDTO.Username));
			user.Otpcode = otpCode;
			user.OtpcreateTime = DateTime.Now;
			var check = await _context.SaveChangesAsync() > 0;
			if (check)
			{
				return Ok();

			}
			return NotFound();

		}
		catch (Exception)
		{
			return NotFound();
		}
	}
}
