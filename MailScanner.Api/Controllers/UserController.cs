namespace MailScanner.Api;


[ApiController]
// [Authorize]
public class UserController(UserManager<User> userManager) : ControllerBase
{
    private readonly UserManager<User> _userManager = userManager;

    [HttpGet]
    [Route("api/users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        return Ok(users);
    }
}
