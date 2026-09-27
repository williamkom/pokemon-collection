using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pokemon.Api.Auth;
using Pokemon.Api.Data;
using Pokemon.Api.Models;
using Pokemon.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// --------------------------
// Database
// --------------------------

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<PokemonDbContext>(
    options =>
    {
        options.UseNpgsql(connectionString);
    });


// --------------------------
// Password hashing
// --------------------------

builder.Services.AddScoped<
    IPasswordHasher<Trainer>,
    PasswordHasher<Trainer>>();


// --------------------------
// JWT configuration
// --------------------------

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(
        builder.Configuration.GetSection(
            JwtOptions.SectionName))
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Issuer),
        "JWT issuer is required.")
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Audience),
        "JWT audience is required.")
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Key)
            && options.Key.Length >= 32,
        "JWT key must contain at least 32 characters.")
    .ValidateOnStart();

var jwtOptions =
    builder.Configuration
        .GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "JWT configuration was not found.");


// --------------------------
// Authentication
// --------------------------

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtOptions.Key)),

                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });
builder.Services.AddHttpClient<IPokemonService, PokemonService>(
    client =>
    {
        client.BaseAddress =
            new Uri("https://pokeapi.co/api/v2/");
        // Set a timeout for the HTTP client to avoid long waits for responses
        client.Timeout = TimeSpan.FromSeconds(10);

        client.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("PokemonCollectionChallenge/1.0");
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<JwtService>();


var app = builder.Build();


// --------------------------
// Middleware
// --------------------------

/*if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}*/

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


// --------------------------
// Development demo users
// --------------------------


if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<PokemonDbContext>();

    await db.Database.MigrateAsync();
}
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("SeedDemoData"))
{
    await DemoDataSeeder.SeedAsync(
        app.Services);
}

app.Run();

public partial class Program;