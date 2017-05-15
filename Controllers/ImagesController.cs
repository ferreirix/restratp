//http://opendata-tr.ratp.fr/wsiv/static/line/m2.gif

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using restratp.Interfaces;

namespace restratp.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private IImageService imageService;
        public ImagesController(IImageService service)
        {
            imageService = service;
        }

        /// <summary>
        /// Endpoint to get the images of lines and networks.
        /// </summary>
        /// <remarks>
        /// For the network's image use :
        /// 1. p_met.gif
        /// 2. p_bus.gif
        /// 3. p_rer.gif
        /// 4. p_tram.gif
        /// 
        /// For the line's image use 'image' value received from api/lines/{networkId}
        /// 
        /// Eg:
        ///
        ///     api/images/p_met.gif
        ///     api/images/m1.gif
        /// </remarks>
        /// <param name="image">The name of the image.</param>
        /// <returns >A byte array with mime type image/gif.</returns>
        [HttpGet("{image}")]
        public async Task<IActionResult> Get(string image)
        {
            image = image.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(image))
            {
                return BadRequest();
            }

            byte[] imageBytes = await imageService.GetImage(image);
            
            return File(imageBytes, "image/gif");
        }
    }
}