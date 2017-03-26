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
    public class LinesController : Controller
    {
        // GET api/lines/metros
        [HttpGet("{networkId}")]
        public async Task<IActionResult> Get(string networkId)
        {
            var service = new WsivPortTypeClient(EndpointConfiguration.WsivSOAP11port_http);

            var network = new Reseau()
            {
                code = networkId
            };

            var line = new Line()
            {
                reseau = network,
                realm = "r"
            };

            var lines = await service.getLinesAsync(line);

            return Json(lines.@return);
        }
    }
}
