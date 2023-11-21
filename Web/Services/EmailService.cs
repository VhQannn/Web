using System.Net;
using System.Net.Mail;
using Web.IRepository;

namespace Web.Services
{
    public class EmailService
    {
        private readonly string baseURL = "https://localhost:7247";
        private readonly IUserRepository _userRepository;
        public EmailService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SendEmailVerifyAsync(string recipient, string username, string OTPCode, string subject)
        {

            var VerifyURL = $"{baseURL}/api/account/verify?email={recipient}&username={username}&otp={OTPCode}";

            string htmlBody = @$"
<html>
<head></head>
<body>
    <p>Chào mừng bạn đã đến với hệ thống Hõ Trợ Học Tập.<br/>
        Vui lòng bấm vào đường dẫn sau để xác nhận email!!!
</p>	
    <a href='{VerifyURL}'>
        <button style='color: white; background-color: blue; padding: 10px 20px; border: none; cursor: pointer;'>Verify Email</button>
    </a>
</body>
</html>";
            var mailMessage = new MailMessage
            {
                From = new MailAddress("hihi@gmail.com"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("hihi@gmail.com", "lcid qgud jvif wmqg");
                smtp.EnableSsl = true;
                mailMessage.To.Add(recipient);
                await smtp.SendMailAsync(mailMessage);
            }


        }

        public async Task SendEmailForgetPassword(string recipient, string username, string OTPCode, string randomPassword)
        {

            var ForgetPassword = $"{baseURL}/api/account/forget-password?email={recipient}&username={username}&otp={OTPCode}";


            string htmlBody = @$"
<html>
<head></head>
<body>
    <p>Mật khẩu mới của bạn là: {randomPassword}</p>
    <p>Bấm vào đường dẫn sau để đổi mật khẩu.</p>	
    <a href='#'>
        <button style='color: white; background-color: blue; padding: 10px 20px; border: none; cursor: pointer;'>Change Password</button>
    </a>
</body>
</html>";
            var mailMessage = new MailMessage
            {
                From = new MailAddress("hihi@gmail.com"),
                Subject = "Forget password",
                Body = htmlBody,
                IsBodyHtml = true
            };

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("hihi@gmail.com", "lcid qgud jvif wmqg");
                smtp.EnableSsl = true;
                mailMessage.To.Add(recipient);
                await smtp.SendMailAsync(mailMessage);
            }


        }
    }
}
