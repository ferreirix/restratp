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
    public class DirectionsController : Controller
    {

        private readonly IMapper mapper;

        public DirectionsController(IMapper mapper)
        {
            this.mapper = mapper;
        }

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
            var directionModel = mapper.Map<Direction[], DirectionModel[]>(directions.@return.directions);
            service.CloseAsync();
            
            return Json(directionModel);
        }
    }
}