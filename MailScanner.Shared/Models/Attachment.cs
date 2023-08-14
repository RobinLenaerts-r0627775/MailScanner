namespace MailScanner.Shared;

public class Attachment : BaseModel
{
    public required string AttachmentName { get; set; }
    public required string Attachmentlocation { get; set; }
}
