using Microsoft.AspNetCore.Identity;

namespace Mailscanner.IdentityServer;

public class UserTest : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
