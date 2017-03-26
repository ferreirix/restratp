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
    public class MissionsController : Controller
    {
        private const int MAX_MISSIONS = 4;


        [HttpGet("{lineId}/from/{fromId}/way/{way:length(1)}")]
        public async Task<IActionResult> GetMissions(string lineId, string fromId, string way)
        {
            var service = new WsivPortTypeClient(EndpointConfiguration.WsivSOAP11port_http);
            var station = new Station()
            {
                id = fromId,
                line = new Line()
                {
                    codeStif = lineId,
                    realm = "r"
                }
            };

            var direction = new Direction()
            {
                sens = way
            };

            var stations = await service.getMissionsNextAsync(station, direction, "", MAX_MISSIONS);

            return Json(stations.@return);
        }
    }
}