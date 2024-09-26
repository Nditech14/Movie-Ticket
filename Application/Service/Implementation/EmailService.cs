using Application.Service.Abstraction;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace Application.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderEmail;

        public EmailService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureCommunicationService:ConnectionString"];
            _emailClient = new EmailClient(connectionString);
            _senderEmail = configuration["AzureCommunicationService:sender"];
        }

        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlContent, string plainTextContent)
        {
            try
            {
                var emailSendOperation = await _emailClient.SendAsync(
                    Azure.WaitUntil.Completed,
                    _senderEmail,
                    recipientEmail,
                    subject,
                    htmlContent,
                    plainTextContent
                );

                return emailSendOperation.HasCompleted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
