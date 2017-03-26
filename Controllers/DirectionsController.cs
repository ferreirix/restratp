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
    public class DirectionsController : Controller
    {
        // GET api/metros/directions/5
        [HttpGet("{lineId}")]
        public async Task<IActionResult> GetDirections(string lineId)
        {
            var service = new WsivPortTypeClient(EndpointConfiguration.WsivSOAP11port_http);
            var line = new Line()
            {
                codeStif = lineId,
                realm = "r"
            };

            var directions = await service.getDirectionsAsync(line);

            return Json(directions.@return);
        }
    }
}