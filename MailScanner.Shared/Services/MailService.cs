namespace MailScanner.Shared;

public class MailService
{
    public readonly MailScannerContext _context;
    public readonly ILogger _logger;
    public readonly IConfiguration _configuration;
    public MailService(MailScannerContext context, ILogger logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public void GetEmails()
    {

        using var client = new ImapClient();
        client.Connect(_configuration["IMAP_HOST"], 993, true);

        client.Authenticate(_configuration["IMAP_USER"], _configuration["IMAP_PASSWORD"]);

        //open archive folder

        var archiveFolder = client.Inbox.GetSubfolder("Archive");
        archiveFolder.Open(FolderAccess.ReadWrite);

        // get last message
        var uids = archiveFolder.Search(SearchQuery.SubjectContains("Invoice").Or(SearchQuery.SubjectContains("Factuur").Or(SearchQuery.SubjectContains("Rekening").Or(SearchQuery.SubjectContains("Kasticket").Or(SearchQuery.BodyContains("Invoice").Or(SearchQuery.BodyContains("Factuur").Or(SearchQuery.BodyContains("Rekening").Or(SearchQuery.BodyContains("kasticket")))))))));

        foreach (var uid in uids)
        {
            var message = client.Inbox.GetSubfolder("Archive").GetMessage(uid);
            var invoice = new Invoice
            {
                Date = message.Date.Date,
                Subject = message.Subject,
                Body = message.TextBody,
                Sender = message.From.ToString(),
                Receiver = message.To.ToString(),

            };
            if (message.Attachments.Any())
            {
                foreach (var attachment in message.Attachments)
                {
                    attachment.WriteTo($"D:\\{attachment.ContentDisposition.FileName}");
                    invoice.Attachments.Add(new Attachment
                    {
                        AttachmentName = attachment.ContentDisposition.FileName,
                        Attachmentlocation = $"D:\\{attachment.ContentDisposition.FileName}"
                    });
                }
                //flag the message as processed

            }
        }
    }
}