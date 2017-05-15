using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using RatpService;
using restratp.Interfaces;

namespace restratp.Services
{
    public class ImageService : BaseService, IImageService
    {
        private const string imgPrefix = "img_";
        private const string ratpBaseImgUrl = "http://opendata-tr.ratp.fr/wsiv/static/line/";

        public ImageService(IMapper mapper,
          IMemoryCache memoryCache,
          WsivPortType ratpService) :
              base(mapper, memoryCache, ratpService)
        {

        }

        public async Task<byte[]> GetImage(string image)
        {
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
                    return null;
                }
            }
            return imageBytes;
        }
    }
}