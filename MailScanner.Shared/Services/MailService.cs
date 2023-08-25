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

    public void Run()
    {

        using var client = new ImapClient();
        client.Connect(_configuration["IMAP_HOST"], 993, true);

        client.Authenticate(_configuration["IMAP_USER"], _configuration["IMAP_PASSWORD"]);

        //open archive folder

        var inboxFolder = client.Inbox;
        inboxFolder.Open(FolderAccess.ReadWrite);

        SearchQuery search = SearchQuery.SubjectContains("qwerqtewrtqwdsagsdga  q");
        if (!_context.Keywords.Any())
        {
            var keyword = new Keyword
            {
                Value = "Kasticket"
            };
            _context.Keywords.Add(keyword);
            _context.SaveChanges();
        }
        foreach (var keyword in _context.Keywords)
        {
            search = search.Or(SearchQuery.SubjectContains(keyword.Value).Or(SearchQuery.BodyContains(keyword.Value)));
        }

        var uids = inboxFolder.Search(search);

        foreach (var uid in uids)
        {
            var message = inboxFolder.GetMessage(uid);
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
                    if (_configuration["FILE_LOCATION"] is null)
                    {
                        _logger.Information("FILE_LOCATION not set, add it to the environment variables please.");
                        return;
                    }
                    var filePath = $"{_configuration["FILE_LOCATION"]}{attachment.ContentDisposition?.FileName}";
                    using var stream = File.Create(filePath);
                    if (attachment is MessagePart)
                    {
                        var part = (MessagePart)attachment;

                        part.Message.WriteTo(stream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        part.Content.DecodeTo(stream);
                    }
                    invoice.Attachments.Add(new Attachment
                    {
                        AttachmentName = attachment.ContentDisposition?.FileName,
                        Attachmentlocation = filePath
                    });
                }
                //move mail to archive/invoices folder
                var invoicesFolder = inboxFolder.GetSubfolder("Archive").GetSubfolder("Invoices");
                inboxFolder.MoveTo(uid, invoicesFolder);
            }
        }
    }
}