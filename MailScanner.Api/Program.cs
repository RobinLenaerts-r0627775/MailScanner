var builder = WebApplication.CreateBuilder(args);

var confBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var configuration = confBuilder.Build();

builder.Services.AddProblemDetails();

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
builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

builder.SetupMySQLDatabaseConnection();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<MailScannerContext>()
    .AddApiEndpoints();
builder.Services.AddControllers();

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

//add authorization with roles
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, ["Admin", "User"]));
    options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
            Enter 'Bearer' [space] and then your token in the text input below.
            Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

builder.Services.AddTransient<IEmailSender, MailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapIdentityApi<User>();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.Run();
