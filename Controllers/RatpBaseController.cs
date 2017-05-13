using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RatpService;

public abstract class RatpBaseController : Controller
{
    protected readonly IMapper mapper;
    protected IMemoryCache cache;
    protected WsivPortType ratpService;

    public RatpBaseController(IMapper mapper,
            IMemoryCache memoryCache,
            WsivPortType ratpService)
    {
        this.mapper = mapper;
        cache = memoryCache;
        this.ratpService = ratpService;
    }

}