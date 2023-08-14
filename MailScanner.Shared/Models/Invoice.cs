namespace MailScanner.Shared;

public class Invoice : BaseModel
{
    [DataType(DataType.EmailAddress)]
    public required string Sender { get; set; }
    [DataType(DataType.EmailAddress)]
    public required string Receiver { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    public required DateTime Date { get; set; }
}
