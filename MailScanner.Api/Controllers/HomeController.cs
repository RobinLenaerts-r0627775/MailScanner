
namespace MailScanner.Api;
[ApiController]
[Route("[controller]")]
public class HomeController : Controller
{
    private readonly ILogger _logger;
    private readonly MailScannerContext _context;
    public HomeController(ILogger logger, MailScannerContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World!");
    }

    // request that gets all invoices requires authorization
    [HttpGet("invoices")]
    [Authorize]
    public IActionResult GetInvoices()
    {
        var invoices = _context.Invoices.ToList();
        return Ok(invoices);
    }
}
