

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


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
builder.Services.AddSingleton<ILogger>(logger);

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

builder.Services.AddDbContext<MailScannerContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<MailScannerContext>()
    .AddApiEndpoints();
builder.Services.AddControllers();

builder.Services.AddAuthentication()
.AddBearerToken(IdentityConstants.BearerScheme);
//add authorization with roles
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var emailSender = new MailSender(configuration, logger);

builder.Services.AddTransient<IEmailSender, MailSender>(provider => emailSender);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePathBase("/robinsapi");

app.MapControllers();

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.Run();
