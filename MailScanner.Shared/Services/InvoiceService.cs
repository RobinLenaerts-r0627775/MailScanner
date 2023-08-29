namespace MailScanner.Shared;

public class InvoiceService : CrudService<Invoice>
{
    private readonly MailScannerContext _context;
    private readonly ILogger _logger;
    public InvoiceService(ILogger logger, MailScannerContext context) : base(context)
    {
        _logger = logger;
        _context = context;
    }
}
