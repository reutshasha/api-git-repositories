using MimeKit;
using MailKit.Net.Smtp;
using BL.Interfaces;

namespace GitRepositoriesApi.Utilities
{
    public class EmailSender : IEmailSender
    {

        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmail(string recipientEmail, string repositoryName)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var server = smtpSettings["Server"];
                var port = int.Parse(smtpSettings["Port"]);
                var senderEmail = smtpSettings["SenderEmail"];
                var senderPassword = smtpSettings["SenderPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("GitHub Search", senderEmail));
                message.To.Add(new MailboxAddress("Recipient Name", recipientEmail));
                message.Subject = $"Information about repository: {repositoryName}";
                message.Body = new TextPart("plain")
                {
                    Text = $"details about the repository: {repositoryName}."
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(server, port, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
