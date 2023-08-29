//Add configuration with user secrets and environment variables

var services = new ServiceCollection();

var confBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var configuration = confBuilder.Build();



//Setup Serilog
var loggerConfig = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}");
if (!string.IsNullOrWhiteSpace(configuration["LogLocation"]))
{
    loggerConfig = loggerConfig.WriteTo.File(configuration["LogLocation"] + "/logfile.log", outputTemplate:
        "[{Timestamp:dd:MM:yyyy - HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day);
}
var logger = loggerConfig.CreateLogger();

//Setup mysql connection
var optionsBuilder = new DbContextOptionsBuilder<MailScannerContext>();
var dbHost = configuration["DB_HOST"];
if (string.IsNullOrEmpty(dbHost))
{
    logger.Information("DB_HOST not set, add it to the environment variables please.");
    return;
}
var dbPort = configuration["DB_PORT"];
if (string.IsNullOrEmpty(dbPort))
{
    logger.Information("DB_PORT not set, using port 3306.");
    dbPort = "3306";
}
var dbUser = configuration["DB_USER"];
if (string.IsNullOrEmpty(dbUser))
{
    logger.Information("DB_USER not set, add it to the environment variables please.");
    return;
}
var dbPassword = configuration["DB_PASSWORD"];
if (string.IsNullOrEmpty(dbPassword))
{
    logger.Information("DB_PASSWORD not set, add it to the environment variables please.");
    return;
}
var connectionString = $"Server={dbHost};Port={dbPort};Database=MailScanner;Uid={dbUser};Pwd={dbPassword};";
optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
var context = new MailScannerContext(optionsBuilder.Options);


logger.Information("Starting migration...");
context.Database.Migrate();
logger.Information("Migration done.");

//Add services
services.AddSingleton<ILogger>(logger);
services.AddDbContext<MailScannerContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
services.AddSingleton<IConfiguration>(configuration);
services.AddTransient<MailService>();

var provider = services.BuildServiceProvider();

//Read mailbox
var mailService = provider.GetService<MailService>();
while (true)
{
    mailService.Run();
    //sleep for 5 minutes
    Thread.Sleep(300000);
}