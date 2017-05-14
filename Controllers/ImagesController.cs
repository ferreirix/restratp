//http://opendata-tr.ratp.fr/wsiv/static/line/m2.gif

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace restratp.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private IMemoryCache cache;
        private const string imgPrefix = "img_";
        private const string ratpBaseImgUrl = "http://opendata-tr.ratp.fr/wsiv/static/line/";
        public ImagesController(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        /// <summary>
        /// Endpoint to get the images of lines and networks.
        /// </summary>
        /// <remarks>
        /// For the network's image use :
        /// 1. p_metro.gif
        /// 2. p_bus.gif
        /// 3. p_rer.gif
        /// 4. p_tram.gif
        /// 
        /// For the line's image use 'image' value received from api/lines/{networkId}
        /// 
        /// Eg:
        ///
        ///     api/images/p_metro.gif
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

            byte[] imageBytes;
            if (!cache.TryGetValue(imgPrefix + image, out imageBytes))
            {
                try
                {
                    using (HttpClient webClient = new HttpClient())
                    {
                        imageBytes = await webClient.GetByteArrayAsync($"{ratpBaseImgUrl}{image}");
                    }
                    cache.Set(imgPrefix + image, imageBytes);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            return File(imageBytes, "image/gif");
        }
    }
}