namespace MailScanner.Api;

public static class WebApplicationBuilderExtension
{

    public static WebApplicationBuilder SetupMySQLDatabaseConnection(this WebApplicationBuilder builder)
    {
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

        var optionsBuilder = new DbContextOptionsBuilder<MailScannerContext>();
        if (string.IsNullOrEmpty(dbHost))
        {
            Log.Information("DB_HOST not set, add it to the environment variables please.");
            return builder;
        }
        if (string.IsNullOrEmpty(dbPort))
        {
            Log.Information("DB_PORT not set, using port 3306.");
            dbPort = "3306";
        }
        if (string.IsNullOrEmpty(dbUser))
        {
            Log.Information("DB_USER not set, add it to the environment variables please.");
            return builder;
        }
        if (string.IsNullOrEmpty(dbPassword))
        {
            Log.Information("DB_PASSWORD not set, add it to the environment variables please.");
            return builder;
        }
        var connectionString = $"Server={dbHost};Port={dbPort};Database=MailScanner;Uid={dbUser};Pwd={dbPassword};";
        builder.Services.AddDbContext<MailScannerContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        return builder;
    }

}
