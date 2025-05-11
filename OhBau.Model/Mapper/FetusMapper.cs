using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Model.Utils;

namespace OhBau.Model.Mapper
{
    public class FetusMapper : Profile
    {
        public FetusMapper()
        {
            CreateMap<CreateFetusRequest, Fetus>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => RandomCodeUtil.GenerateRandomCode(10)))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));

            CreateMap<Fetus, CreateFetusResponse>();

            CreateMap<Fetus, GetFetusResponse>();
        }
    }
}
