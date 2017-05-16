using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ImageSharp;
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
                var prominentColor = GetColor(image)
                                        .Where(color => color != (uint)LineColor.Black && color != (uint)LineColor.White) //not black or white
                                        .GroupBy(color => color)
                                        .OrderByDescending(grp => grp.Count())
                                        .Select(grp => grp.Key).FirstOrDefault();

                var x = image.Pixels.FirstOrDefault(p => p.PackedValue == prominentColor);

                imgRGB = $"rgb({x.R},{x.G},{x.B})";
            }

            return imgRGB;
        }

        private IEnumerable<uint> GetColor(Image<Rgba32> img)
        {
            var count = img.Pixels.Length;
            foreach (var pixel in img.Pixels)
            {
                yield return pixel.PackedValue;
            }
        }
    }
}