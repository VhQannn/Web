using System.Net;
using System.Net.Mail;

namespace Web.Services
{
	public class EmailService
	{

		public EmailService()
		{

		}

		public async Task SendEmailAsync(string recipient, string username, string OTPCode, string subject)
		{
			var baseURL = "https://localhost:7247";
			var VerifyURL = $"{baseURL}/api/account/verify?email={recipient}&username={username}&otp={OTPCode}";


			string htmlBody = @$"
<html>
<head></head>
<body>
    <p>Click the button below to verify your email address:</p>	
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
	}
}
