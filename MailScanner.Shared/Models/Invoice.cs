namespace MailScanner.Shared;

public class Invoice : BaseModel
{
    [DataType(DataType.EmailAddress)]
    public string sender { get; set; }
    [DataType(DataType.EmailAddress)]
    public string receiver { get; set; }
    public string subject { get; set; }
    public string body { get; set; }
    public string attachmentlocation { get; set; }
    public string attachmentName { get; set; }
}
