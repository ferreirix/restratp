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
    public class DirectionsController : RatpBaseController
    {
        private const string directionsPrefix = "dir_";

        public DirectionsController(IMapper mapper,
            IMemoryCache memoryCache,
            WsivPortType ratpService) :
                base(mapper, memoryCache, ratpService)
        { }

        // GET api/metros/directions/5
        [HttpGet("{lineId}")]
        public async Task<IActionResult> GetDirections(string lineId)
        {
            lineId = lineId.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(lineId))
            {
                return BadRequest();
            }
            DirectionModel[] directionsModel;
            if (!cache.TryGetValue(directionsPrefix + lineId, out directionsModel))
            {
                var line = new Line()
                {
                    codeStif = lineId,
                    realm = "r"
                };

                var directions = await ratpService.getDirectionsAsync(new getDirectionsRequest(line));
                directionsModel = mapper.Map<Direction[], DirectionModel[]>(directions.@return.directions);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                // Save data in cache.
                cache.Set(directionsPrefix + lineId, directionsModel, cacheEntryOptions);
            }
            return Json(directionsModel);
        }
    }
}