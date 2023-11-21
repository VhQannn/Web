using System.Net;
using System.Net.Mail;
namespace Web.Services
{
	public class EmailService
	{

		public EmailService()
		{

		}

		public async Task SendEmailAsync(string recipient, string subject, string body)
		{
			var mailMessage = new MailMessage
			{
				From = new MailAddress("luab00626@gmail.com"),
				Subject = subject,
				Body = body
			};

			using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
			{
				smtp.UseDefaultCredentials = false;
				smtp.Credentials = new NetworkCredential("luab00626@gmail.com", "lcid qgud jvif wmqg");
				smtp.EnableSsl = true;
				mailMessage.To.Add(recipient);
				await smtp.SendMailAsync(mailMessage);
			}


		}
	}
}
