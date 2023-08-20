namespace MailScanner.Shared;

public class BaseModel
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.MinValue;
    public DateTime DeletedAt { get; set; } = DateTime.MinValue;
}
