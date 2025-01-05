namespace BL.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmail(string recipientEmail, string repositoryName);
    }
}
