using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using Taxi;
using Taxi.API.Server.DTO;
using Taxi.API.Server.Models;
using Taxi.API.Server.Services;

var key = new SymmetricSecurityKey(
    RandomNumberGenerator.GetBytes(32));

var validation = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = "TaxiJWT",

    ValidateAudience = true,
    ValidAudience = "TaxiApiJWT",

    ValidateIssuerSigningKey = true,
    IssuerSigningKey = key,

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,

    NameClaimType = "name",
    RoleClaimType = "role"
};

var builder = WebApplication.CreateBuilder(args);
var hasher = new PasswordHasher<User>();
// Add services to the container.
builder.Services.
    AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = true;
        options.TokenValidationParameters = validation;
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<TaxiBaseContext>();

var app = builder.Build();
DotEnv.Load();
app.UseAuthentication();
app.UseAuthorization();


// Configure the HTTP request pipeline.
var cache = AppCache.Cache;
var httpClient = new HttpClient();
var geocodeService = new GeocodeService(httpClient);
var routingService = new RoutingService(httpClient);

app.MapGet("/geocode", geocodeService.GeocodeAsync);
app.MapGet("/getRoute", routingService.GetRouteAsync);
app.MapPost("/auth/login", async (TaxiBaseContext context, LoginRequest request) =>
{
    var account = await context.Users.FirstOrDefaultAsync(
        a => a.Login == request.login);

    if (account == null ||
        string.IsNullOrWhiteSpace(request.password))
        return Results.Challenge();

    var result = hasher.VerifyHashedPassword(
        account, account.PasswordHashe, request.password);

    if (result == PasswordVerificationResult.Failed)
        return Results.Challenge();

    return Results.Ok(new
    {
        access_token = CreateToken(account),
        token_type = "Bearer"
    });
}).AllowAnonymous();

app.MapPost("/auth/register", async (TaxiBaseContext context, RegisterRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.login))
        return Results.BadRequest(new { message = "Логин обязателен" });

    if (string.IsNullOrWhiteSpace(request.password))
        return Results.BadRequest(new { message = "Пароль обязателен" });

    if (string.IsNullOrWhiteSpace(request.email))
        return Results.BadRequest(new { message = "Email обязателен" });

    if (string.IsNullOrWhiteSpace(request.lastName) || string.IsNullOrWhiteSpace(request.middleName) || string.IsNullOrWhiteSpace(request.firstName))
        return Results.BadRequest(new { message = "ФИО обязателен" });

    var exists = await context.Users.AnyAsync(u => u.Login == request.login);
    if (exists) return Results.Conflict(new { message = "Логин уже занят" });

    var hasher = new PasswordHasher<User>();

    var user = new User
    {
        Login = request.login,
        RoleId = 1,
        LastName = request.lastName,
        FirstName = request.firstName,
        MiddleName = request.middleName,
        Email = request.email,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    user.PasswordHashe = hasher.HashPassword(user, request.password);



    context.Users.Add(user);
    await context.SaveChangesAsync();

    string fio = $"{user.FirstName}{user.MiddleName}{user.LastName}";

    return Results.Created($"/api/users/{user.Id}", new
    {
        user.Id,
        user.Login,
        fio,
        user.RoleId
    });
}).AllowAnonymous();

//app.MapPost("/register", )

app.Run();
string CreateToken(User account)
{
    var claims = new[]
    {
        new Claim("sub", account.Id.ToString()),
        new Claim("name", account.Login),
        new Claim("role", account.RoleId.ToString())
    };

    var token = new JwtSecurityToken(
        issuer: "TaxiJWT",
        audience: "TaxiApiJWT",
        claims: claims,
        //expires: DateTime.UtcNow.AddMinutes(15),
        signingCredentials: new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(token);
}
