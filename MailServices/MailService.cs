using System.Net;
using System.Net.Mail;

namespace Project.MailServices
{
    // DONE
    public class MailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendMail(string email, string subject, string body)
        {
            string username = _configuration["SmtpSettings:Username"];
            string appPassword = _configuration["SmtpSettings:AppPassword"];
            string host = _configuration["SmtpSettings:Host"];
            int port = int.Parse(_configuration["SmtpSettings:Port"]);

            // Retrieve SMTP credentials from configuration
            NetworkCredential networkCredential = new NetworkCredential(username, appPassword);

            try
            {
                // Configure the SMTP client
                using SmtpClient client = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = networkCredential,
                };

                // Create the email message
                MailAddress from = new MailAddress(username, "Dotnet Project");
                MailAddress to = new MailAddress(email);
                using MailMessage message = new MailMessage(from, to)
                {
                    Subject = "Demo Only: " + subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                await client.SendMailAsync(message);
                return true; // Mail sent successfully
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send OTP: {ex.Message}");
                return false; // Mail sending failed
            }
        }
    }
}
