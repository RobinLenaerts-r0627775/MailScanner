using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MailScanner.Shared.DB;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MailScannerContextConnection") ?? throw new InvalidOperationException("Connection string 'MailScannerContextConnection' not found.");

var confBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var configuration = confBuilder.Build();

// Add services to the container.

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

builder.Services.AddSingleton<ILogger>(logger);

builder.Services.AddScoped<MailService>();

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorizationBuilder();

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

builder.Services.AddDbContext<MailScannerContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MailScannerContext>();

builder.Services.AddIdentityCore<User>()
.AddEntityFrameworkStores<MailScannerContext>()
.AddApiEndpoints();

var app = builder.Build();

app.MapIdentityApi<User>();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

