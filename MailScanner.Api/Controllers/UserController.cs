namespace MailScanner.Api;


[ApiController]
public class UserController : ControllerBase
{

    private readonly UserManager<User> _userManager;


    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Route("api/users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        return Ok(users);
    }
}
