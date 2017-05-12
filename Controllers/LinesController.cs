﻿using System;
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
    public class LinesController : Controller
    {
        private readonly IMapper mapper;
        private IMemoryCache cache;

        public LinesController(IMapper mapper, IMemoryCache memoryCache)
        {
            this.mapper = mapper;
            cache = memoryCache;
        }

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
        /// Eg: api/lines/metro              
        /// </remarks>
        /// <param name="networkId">The id of the network.</param>
        /// <returns >All the lines of the requested network.</returns>
        [HttpGet("{networkId}")]
        [Produces(typeof(LineModel))]
        [SwaggerResponse(200, Type = typeof(LineModel))]
        public async Task<IActionResult> Get(string networkId)
        {
            networkId = networkId.Trim().ToLower();

            getLinesResponse lines;
            if (!cache.TryGetValue(networkId, out lines))
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

                lines = await service.getLinesAsync(line);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(24));

                // Save data in cache.
                cache.Set(networkId, lines, cacheEntryOptions);
            }

            var lineModel = mapper.Map<Line[], LineModel[]>(lines.@return);

            return Json(lineModel);
        }
    }
}
