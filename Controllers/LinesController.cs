using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RatpService;
using restratp.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using restratp.Interfaces;

namespace restratp.Controllers
{

    [Route("api/[controller]")]
    public class LinesController : Controller
    {
        private ILineService lineService;
        public LinesController(ILineService service)
        {
            lineService = service;
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

            var lines = await lineService.GetNetworkLines(networkId);
            return Json(lines);
        }




        /// <summary>
        /// Endpoint to get the color of one line.
        /// </summary>
        /// <remarks>
        /// The available network ids are :
        /// 1. metro
        /// 2. bus
        /// 3. rer
        /// 4. tram
        ///
        /// The available lineIds are returned from api/lines/{networkId}
        ///
        /// Eg:
        ///
        ///     api/lines/metro/line/100110001/color           
        /// </remarks>
        /// <param name="networkId">The id of the network.</param>
        /// <param name="lineId">The id of the line.</param>        
        /// <returns >The color in rgb format eg: rgb(109, 94, 207) .</returns>
        [Produces(typeof(string))]
        [SwaggerResponse(200, Type = typeof(string))]
        [HttpGet("{networkId:minlength(3)}/line/{lineId}/color")]
        [SwaggerOperation(Tags = new[] { "01. Lines" })]
        public async Task<IActionResult> GetLineColor(string networkId, string lineId)
        {
            networkId = networkId.Trim().ToLower();
            lineId = lineId.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(networkId) || string.IsNullOrWhiteSpace(lineId))
            {
                return BadRequest();
            }

            string imgRGB = await lineService.GetLineColor(networkId, lineId);

            return Ok(imgRGB);
        }
    }
}
