using MailKit.Net.Smtp;
using MimeKit;

namespace Stuffed_Animal_Shop.Services
{
    public class EmailService
    {
        public void SendEmail(string to, string subject, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Thai meo", "thaitran15072003!=@gmail.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();

			// Read HTML content from file
			string htmlBody = File.ReadAllText("Views/Shared/_EmailTemplate.cshtml");

			bodyBuilder.HtmlBody = string.Format(htmlBody, password);

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);

                // Note: Since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("thaitran15072003@gmail.com", "sawu pcas bbbg kvjc");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
