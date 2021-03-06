using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SixLabors.ImageSharp;
using Microsoft.Extensions.Caching.Memory;
using RatpService;
using restratp.Interfaces;
using restratp.Models;
using System.Linq;

namespace restratp.Services
{
    public class LineService : BaseService, ILineService
    {
        private IImageService imageService;

        public LineService(IMapper mapper,
           IMemoryCache memoryCache,
           WsivPortType ratpService,
           IImageService imageService) :
               base(mapper, memoryCache, ratpService)
        {
            this.imageService = imageService;
        }

        public async Task<LineModel[]> GetNetworkLines(string networkId)
        {
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

            return lines;
        }


        public async Task<string> GetLineColor(string networkId, string lineId)
        {
            string imgRGB = string.Empty;

            LineModel[] lines = await GetNetworkLines(networkId);

            var line = lines.FirstOrDefault(l => l.Id == lineId);
            if (line != null)
            {
                var imageBytes = await imageService.GetImage(line.Image);
                var image = Image.Load(imageBytes);
                var prominentColor = GetPixelsColor(image)
                                        .Where(color => color != (uint)LineColor.Black && color != (uint)LineColor.White) //not black or white
                                        .GroupBy(color => color)
                                        .OrderByDescending(grp => grp.Count())
                                        .Select(grp => grp.Key).FirstOrDefault();

                var rgbColor = new Rgba32(prominentColor);

                imgRGB = $"rgb({rgbColor.R},{rgbColor.G},{rgbColor.B})";
            }

            return imgRGB;
        }

        private IEnumerable<uint> GetPixelsColor(Image<Rgba32> img)
        {
            var heigth = img.Height;
            var length = img.Width;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < heigth; j++)
                {
                    yield return img[i, j].PackedValue;
                }
            }
        }
    }
}