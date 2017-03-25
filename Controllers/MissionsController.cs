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
          // GET api/metros/stations/5
        [HttpGet("{lineId}/from/{fromId}")] //  to/{toId}
        public async Task<IActionResult> GetMissions(string lineId, string fromId) //, string toId
        {
            var service = new WsivPortTypeClient(EndpointConfiguration.WsivSOAP11port_http);        
            var station = new Station()
            {
                id = fromId,
                line = new Line()
                {
                    id = lineId
                }
            };

            var direction = new Direction();
            direction.sens = "A";

            var stations = await service.getMissionsNextAsync(station, direction, "", 4);

            return Json(stations.@return);
        }
    }
}