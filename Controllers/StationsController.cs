using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatpService;
using restratp.Models;
using static RatpService.WsivPortTypeClient;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace restratp.Controllers
{
    [Route("api/[controller]")]
    public class StationsController : RatpBaseController
    {
        private const string stationsPrefix = "sta_";

        public StationsController(IMapper mapper,
            IMemoryCache memoryCache,
            WsivPortType ratpService) :
                base(mapper, memoryCache, ratpService)
        { }

        /// <summary>
        /// Endpoint to get all the stations of a specific line.
        /// </summary>
        /// <remarks>
        /// The available lineIds are returned from api/lines/{networkId}
        ///
        /// Eg:
        ///
        ///     api/stations/100110001
        /// </remarks>
        /// <param name="lineId">The id of the line.</param>
        /// <returns >All the stations of the requested line.</returns>
        [HttpGet("{lineId}")]
        [Produces(typeof(StationModel))]
        [SwaggerResponse(200, Type = typeof(StationModel))]
        [SwaggerOperation(Tags = new[] { "03. Stations" })]
        public async Task<IActionResult> GetStations(string lineId)
        {
            lineId = lineId.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(lineId))
            {
                return BadRequest();
            }

            StationModel[] stationsModel;
            if (!cache.TryGetValue(stationsPrefix + lineId, out stationsModel))
            {
                var station = new Station()
                {
                    line = new Line()
                    {
                        codeStif = lineId,
                        realm = "r"
                    }
                };
                var stationRequest = new getStationsRequest(station, null, null, 0, true);
                var stations = await ratpService.getStationsAsync(stationRequest);

                stationsModel = mapper.Map<Station[], StationModel[]>(stations.@return.stations);
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                // Save data in cache.
                cache.Set(stationsPrefix + lineId, stationsModel, cacheEntryOptions);
            }
            return Json(stationsModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{stationId}/coordinates")]
        public async Task<IActionResult> GetStationCoordinates(string stationId)
        {
            var service = new WsivPortTypeClient(EndpointConfiguration.WsivSOAP11port_http);
            var geoP = new RatpService.GeoPoint()
            {
                id = stationId
            };

            var point = await service.getGeoPointsAsync(geoP, 0);
            return Json(point);
        }

    }
}