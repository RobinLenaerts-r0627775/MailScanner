namespace MailScanner.Shared;

public class MailSender : IEmailSender
{
    public readonly IConfiguration _configuration;
    public readonly ILogger _logger;

    public MailSender(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        //implement mailing

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_configuration["SMTP_USER"]));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };
        try
        {
            //send email
            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["SMTP_HOST"], int.Parse(_configuration["SMTP_PORT"] ?? "465"));
            smtp.Authenticate(_configuration["SMTP_USER"], _configuration["SMTP_PASSWORD"]);
            var resp = smtp.Send(message);
            smtp.Disconnect(true);
        }
        catch (Exception ex)
        {
            throw;
        }
        return Task.CompletedTask;
    }


}
