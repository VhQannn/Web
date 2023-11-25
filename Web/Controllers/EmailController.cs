using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web.DbConnection;
using Web.DTOs;
using Web.IRepository;
using Web.Services;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly WebContext _context;
    private readonly IUserRepository _userRepository;
    public EmailController(EmailService emailService, WebContext context, IUserRepository userRepository)
    {
        _emailService = emailService;
        _context = context;
        _userRepository = userRepository;
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
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(verifyEmailDTO.Email) && x.Username.Equals(verifyEmailDTO.Username));
            if (user.IsVerify.Value)
            {
                return BadRequest("Tài khoản đã được xác nhận");
            }
            var otpCode = GetOTP();
            await _emailService.SendEmailVerifyAsync(verifyEmailDTO.Email, verifyEmailDTO.Username, otpCode, "Verify Email");
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

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword(VerifyPasswordDTO verifyPassswordDTO)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(verifyPassswordDTO.Email) && x.Username.Equals(verifyPassswordDTO.Username));
            if (!(user.IsVerify.Value))
            {
                return BadRequest("Tài khoản chưa được xác nhận");
            }
            var otpCode = GetOTP();
            user.Otpcode = otpCode;
            user.OtpcreateTime = DateTime.Now;

            string randompass = _userRepository.RandomPassword();
            var changePass = _userRepository.ChangePassword(verifyPassswordDTO.Username, randompass);
            if (changePass == null)
            {
                return BadRequest("Có lỗi xảy ra.");
            }
            await _emailService.SendEmailForgetPassword(verifyPassswordDTO.Email, verifyPassswordDTO.Username, otpCode, randompass);
            return Ok();

        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}
