using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatpService;
using restratp.Models;
using static RatpService.WsivPortTypeClient;

namespace restratp.Controllers
{
    [Route("api/[controller]")]
    public class StationsController : Controller
    {
        private readonly IMapper mapper;

        public StationsController(IMapper mapper)
        {
            this.mapper = mapper;
        }

        
        [HttpGet("{lineId}")]
        public async Task<IActionResult> GetStations(string lineId)
        {
            var service = new WsivPortTypeClient(EndpointConfiguration.WsivSOAP11port_http);
            var station = new Station()
            {
                line = new Line()
                {
                    codeStif = lineId,
                    realm = "r"
                }
            };

            var stations = await service.getStationsAsync(station, null, null, 0, true);

            var stationModel = mapper.Map<Station[], StationModel[]>(stations.@return.stations);
            service.CloseAsync();

            return Json(stationModel);
        }

        [HttpGet("{stationId}/coordinates")]
        public async Task<IActionResult> GetStationCoordinates(string stationId)
        {
            var service = new WsivPortTypeClient(EndpointConfiguration.WsivSOAP11port_http);
            var geoP = new RatpService.GeoPoint()
            {
                id = stationId
            };

            var point = await service.getGeoPointsAsync(geoP,0);
            return Json(point);
        }

    }
}