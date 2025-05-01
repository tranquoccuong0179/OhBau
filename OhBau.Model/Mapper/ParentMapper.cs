using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.Parent;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Model.Utils;

namespace OhBau.Model.Mapper
{
    public class ParentMapper : Profile
    {
        public ParentMapper()
        {
            CreateMap<RegisterParentRequest, Parent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.AccountId, opt => opt.Ignore());

            CreateMap<Parent, RegisterParentResponse>();
        }
    }
}
