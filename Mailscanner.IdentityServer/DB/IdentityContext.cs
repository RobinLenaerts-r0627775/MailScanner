namespace Mailscanner.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext<UserTest>(options)
{
    //seed test users
}