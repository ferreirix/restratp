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

namespace restratp.Controllers
{
    [Route("api/[controller]")]
    public class StationsController : Controller
    {
        private readonly IMapper mapper;
        private IMemoryCache cache;
        private WsivPortType ratpService;
        private const string stationsPrefix = "sta_";

        public StationsController(IMapper mapper,
            IMemoryCache memoryCache,
            WsivPortType ratpService)
        {
            this.mapper = mapper;
        }


        [HttpGet("{lineId}")]
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
                    .SetSlidingExpiration(TimeSpan.FromHours(24));
                // Save data in cache.
                cache.Set(stationsPrefix + lineId, stationsModel, cacheEntryOptions);
            }
            return Json(stationsModel);
        }

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