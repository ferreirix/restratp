using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RatpService;
using restratp.Models;
using static RatpService.WsivPortTypeClient;

namespace restratp.Controllers
{
    [Route("api/[controller]")]
    public class MissionsController : RatpBaseController
    {
        public MissionsController(IMapper mapper,
            IMemoryCache memoryCache,
            WsivPortType ratpService) :
                base(mapper, memoryCache, ratpService)
        { }

        private const int MAX_MISSIONS = 4;


        [HttpGet("{lineId}/from/{fromId}/way/{way:length(1)}")]
        public async Task<IActionResult> GetMissions(string lineId, string fromId, string way)
        {
            lineId = lineId.Trim().ToLower();
            fromId = fromId.Trim().ToLower();
            way = way.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(lineId) ||
                string.IsNullOrWhiteSpace(fromId) ||
                string.IsNullOrWhiteSpace(way)
            )
            {
                return BadRequest();
            }

            var cacheItem = $"{lineId}.{fromId}.{way}";
            getMissionsNextResponse missions;
            if (!cache.TryGetValue(cacheItem, out missions))
            {
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

                var missionsRequest = new getMissionsNextRequest(station, direction, "", MAX_MISSIONS);
                missions = await ratpService.getMissionsNextAsync(missionsRequest);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(5));
                // Save data in cache.
                cache.Set(cacheItem, missions, cacheEntryOptions);
            }
            return Json(missions);
        }
    }
}