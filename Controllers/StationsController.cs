using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RatpService;
using restratp.Models;
using static RatpService.WsivPortTypeClient;

namespace restratp.Controllers
{
    [Route("api/[controller]")]
    public class StationsController : Controller
    {

        // GET api/stations/5
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

            return Json(stations.@return);
        }

    }
}