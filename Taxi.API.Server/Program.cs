using dotenv.net;
using Taxi;
using Taxi.API.Server.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();
DotEnv.Load();

// Configure the HTTP request pipeline.
var cache = AppCache.Cache;
var httpClient = new HttpClient();
var geocodeService = new GeocodeService(httpClient);
var routingService = new RoutingService(httpClient);

app.MapGet("/geocode", geocodeService.GeocodeAsync);
app.MapGet("/getRoute", routingService.GetRouteAsync);


app.Run();
