using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Taxi;
using Taxi.API.Server;
using Taxi.API.Server.Models;
using Taxi.API.Server.Services;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Error Jwt .env");

var signKey = jwt.GetSymmetricSecurityKey();

var validation = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = jwt.Issuer,

     ValidateAudience = true,
    ValidAudience = jwt.Audience,

    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signKey,

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,

    NameClaimType = "name",
    RoleClaimType = "role"
};


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = true;
        options.TokenValidationParameters = validation;
    });


builder.Services.AddAuthorization();

builder.Services.AddScoped<Authorization>();

builder.Services.AddControllers();
builder.Services.AddDbContext<TaxiBaseContext>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


// Configure the HTTP request pipeline.
var httpClient = new HttpClient();
var geocodeService = new GeocodeService(httpClient);
var routingService = new RoutingService(httpClient);

app.MapGet("/geocode", geocodeService.GeocodeAsync);
app.MapGet("/getRoute", routingService.GetRouteAsync);

app.MapControllers();



//app.MapPost("/register", )

app.Run();
