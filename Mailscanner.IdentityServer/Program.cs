
using System.Security.Claims;
using Mailscanner.IdentityServer;
using MailScanner.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
var builder = WebApplication.CreateBuilder(args);


var confBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var configuration = confBuilder.Build();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add user manager and signin manager to DI
builder.Services.AddIdentity<UserTest, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();



builder.Services.AddControllers();

var connectionString = "Server=localhost;Port=3306;Database=IdentityTest;Uid=root;Pwd=Klp-246135;" ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
        .AddCore(options =>
        {
            // Configure OpenIddict to use the Entity Framework Core stores and models.
            // Note: call ReplaceDefaultEntities() to replace the default entities.
            options.UseEntityFrameworkCore()
                .UseDbContext<IdentityContext>();
        })

        // Register the OpenIddict server components.
        .AddServer(options =>
        {
            // Enable the token endpoint.
            options.SetTokenEndpointUris("connect/token");
            options.SetIntrospectionEndpointUris("connect/introspect");
            options.SetIssuer("https://localhost:7019");
            // Enable the client credentials flow.
            options.AllowPasswordFlow();

            options.RegisterClaims(OpenIddictConstants.Claims.Name);


            options.AcceptAnonymousClients();

            // Register the signing and encryption credentials.
            options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();

            // Register the ASP.NET Core host and configure the ASP.NET Core options.
            options.UseAspNetCore()
                .EnableTokenEndpointPassthrough();

            options.RegisterClaims(["name", "email", "role"]);
        })

        // Register the OpenIddict validation components.
        .AddValidation(options =>
        {
            options.SetIssuer("https://localhost:7019");
            // Import the configuration from the local OpenIddict server instance.
            // options.UseLocalServer();
            // Register the ASP.NET Core host.
            options.UseAspNetCore();
            options.UseIntrospection()
            .SetClientId("console") // Set the client ID here
            .SetClientSecret("388D45FA-B36B-4988-BA59-B187D329C207")
            .SetIssuer("https://localhost:7019");
            options.UseSystemNetHttp();

            // make sure all 
        });

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapDefaultControllerRoute();
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
