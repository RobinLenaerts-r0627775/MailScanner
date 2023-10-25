namespace MailScanner.Api;


[ApiController]
[Route("[controller]")]
public class UsersController(UserManager<User> userManager, IEmailSender emailSender) : ControllerBase
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        return Ok(users);
    }

    //send 2fa email
    [HttpPost]
    [Route("send2fa")]
    public async Task<IActionResult> Send2faEmail([FromBody] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }
        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        await _emailSender.SendEmailAsync(email, "2FA", $"Your 2FA token is: {token}");
        return Ok();
    }
}
