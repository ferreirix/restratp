using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using RatpService;

namespace restratp.Services
{
    public abstract class BaseService
    {
        protected readonly IMapper mapper;
        protected IMemoryCache cache;
        protected WsivPortType ratpService;

        public BaseService(IMapper mapper,
                IMemoryCache memoryCache,
                WsivPortType ratpService)
        {
            this.mapper = mapper;
            cache = memoryCache;
            this.ratpService = ratpService;
        }
    }
}