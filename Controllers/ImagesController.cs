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

        [HttpGet("{image}")]
        [Produces(typeof(byte[]))]
        [SwaggerResponse(200, Type = typeof(byte[]))]
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