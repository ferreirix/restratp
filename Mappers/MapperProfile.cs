using System;
using AutoMapper;
using AutoMapper.Configuration;
using RatpService;
using restratp.Models;

namespace restratp.Mappers
{
    public class MappingProfile : MapperConfigurationExpression
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Line, LineModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.codeStif))
                .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.id));

            CreateMap<Direction, DirectionModel>()
                .ForMember(dest => dest.Way, opt => opt.MapFrom(src => src.sens));


            CreateMap<Station, StationModel>();
        }
    }
}