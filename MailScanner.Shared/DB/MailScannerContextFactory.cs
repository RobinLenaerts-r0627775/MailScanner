namespace MailScanner.Shared;

public class MailScannerContextFactory : IDesignTimeDbContextFactory<MailScannerContext>
{
    private readonly IConfiguration _configuration;
    public MailScannerContextFactory()
    {
        var builder = new ConfigurationBuilder()
        .AddUserSecrets(GetType().Assembly, true)
        .AddEnvironmentVariables();
        _configuration = builder.Build();
    }
    public MailScannerContext CreateDbContext(string[] args)
    {
        var connectionString = $"Server={_configuration["DB_HOST"]};Port={_configuration["DB_PORT"] ?? "3306"};Database=MailScanner;Uid={_configuration["DB_USER"]};Pwd={_configuration["DB_PASSWORD"]};";
        var optionsBuilder = new DbContextOptionsBuilder<MailScannerContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new MailScannerContext(optionsBuilder.Options);
    }
}
