using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;
using Taxi.Core.Models.Map;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/geo")]
    public class MapController : ControllerBase 
    {
        private readonly RoutingService _routingService;
        private readonly GeocodeService _geocodeService;

        public MapController(RoutingService routingService, GeocodeService geocodeService)
        {
            _routingService = routingService;
            _geocodeService = geocodeService;
        }

        [HttpGet("GetAddressGeocode")]
        public async Task<IActionResult> GetAddressToGeocodeAsync(string address)
        {
            return Ok(
                await _geocodeService.GeocodeAsync(address)
                
                );
        }

        //[HttpGet("GetAddressFromGeocode")]
        //public async Task<IActionResult> GetAddressFromGeocodeAsync(string address_coord)
        //{
        //    return Ok(
        //        await _geocodeService.GeocodeAsync(address)

        //        );
        //}
        [HttpPost("GetRoute")]
        public async Task<IActionResult> GetRoutAsync(RouteRequest routeRequest)
        {
            return Ok(await _routingService.GetRouteAsync(
                routeRequest.from.Lat,
                routeRequest.from.Lon,
                routeRequest.from.Address,
                routeRequest.to.Lat,
                routeRequest.to.Lon,
                routeRequest.to.Address,
                useMapbox: true
                ));
        }
    }
}
