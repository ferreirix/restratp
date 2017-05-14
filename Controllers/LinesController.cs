using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RatpService;
using restratp.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using static RatpService.WsivPortTypeClient;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace restratp.Controllers
{
    
    [Route("api/[controller]")]
    public class LinesController : RatpBaseController
    {
       
        public LinesController(IMapper mapper,
            IMemoryCache memoryCache,
            WsivPortType ratpService) :
                base(mapper, memoryCache, ratpService)
        { }

        /// <summary>
        /// Endpoint to get all the lines of a specific network.
        /// </summary>
        /// <remarks>
        /// The available network ids are :
        /// 1. metro
        /// 2. bus
        /// 3. rer
        /// 4. tram
        ///
        /// Eg:
        ///
        ///     api/lines/metro              
        /// </remarks>
        /// <param name="networkId">The id of the network.</param>
        /// <returns >All the lines of the requested network.</returns>
        [HttpGet("{networkId:minlength(3)}")]
        [Produces(typeof(LineModel))]
        [SwaggerResponse(200, Type = typeof(LineModel))]
        [SwaggerOperation(Tags = new[] { "01. Lines" })]
        public async Task<IActionResult> Get(string networkId)
        {
            networkId = networkId.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(networkId))
            {
                return BadRequest();
            }

            LineModel[] lines;
            if (!cache.TryGetValue(networkId, out lines))
            {
                var line = new Line()
                {
                    reseau = new Reseau() { code = networkId },
                    realm = "r"
                };

                var linesResponse = await ratpService.getLinesAsync(new getLinesRequest(line));
                lines = mapper.Map<Line[], LineModel[]>(linesResponse.@return);
                
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                // Save data in cache.
                cache.Set(networkId, lines, cacheEntryOptions);
            }

            return Json(lines);
        }
    }
}
